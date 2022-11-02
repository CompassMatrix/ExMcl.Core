using Microsoft;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Inited
{

    class DestAndOri
    {
        /// <summary>
        /// Hook������
        /// </summary>
        public MethodBase HookMethod { get; set; }

        /// <summary>
        /// Ŀ�귽����ԭʼ����
        /// </summary>
        public MethodBase OriginalMethod { get; set; }

        public IMethodHook Obj;
    }

    public class MethodHook
    {
        static BindingFlags AllFlag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
        static bool installed = false;
        static System.Collections.Generic.List<DestAndOri> destAndOris = new System.Collections.Generic.List<DestAndOri>();

        /// <summary>
        /// ��װ������
        /// </summary>
        public static int Install()
        {
            if (installed)
                return 0;
            installed = true;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            System.Collections.Generic.IEnumerable<IMethodHook> monitors = assemblies.SelectMany(t => t.GetImplementedObjectsByInterface<IMethodHook>());

            foreach (var monitor in monitors)
            {
                var all = monitor.GetType().GetMethods(AllFlag);
                var hookMethods = all.Where(t => t.CustomAttributes.Any(a => typeof(HookMethodAttribute).IsAssignableFrom(a.AttributeType)));
                var originalMethods = all.Where(t => t.CustomAttributes.Any(a => typeof(OriginalMethodAttribute).IsAssignableFrom(a.AttributeType))).ToArray();

                var destCount = hookMethods.Count();
                foreach (var hookMethod in hookMethods)
                {
                    DestAndOri destAndOri = new DestAndOri();
                    destAndOri.Obj = monitor;
                    destAndOri.HookMethod = hookMethod;
                    if (destCount == 1)
                    {
                        destAndOri.OriginalMethod = originalMethods.FirstOrDefault();
                    }
                    else
                    {
                        var originalMethodName = hookMethod.GetCustomAttribute<HookMethodAttribute>().GetOriginalMethodName(hookMethod);

                        destAndOri.OriginalMethod = FindMethod(originalMethods, originalMethodName, hookMethod, assemblies);
                    }

                    destAndOris.Add(destAndOri);
                }
            }

            InstallInternal(true, assemblies);
            return 1;
        }

        private static void InstallInternal(bool isInstall, Assembly[] assemblies)
        {
            foreach (var detour in destAndOris)
            {
                var hookMethod = detour.HookMethod;
                var hookMethodAttribute = hookMethod.GetCustomAttribute<HookMethodAttribute>();

                //��ȡ��ǰ�����еĻ�������
                var typeName = hookMethodAttribute.TargetTypeFullName;
                if (hookMethodAttribute.TargetType != null)
                {
                    typeName = hookMethodAttribute.TargetType.FullName;
                }
                var type = TypeResolver(typeName, assemblies);
                if (type != null && !assemblies.Contains(type.Assembly))
                {
                    type = null;
                }

                //��ȡ����
                var methodName = hookMethodAttribute.GetTargetMethodName(hookMethod);
                MethodBase rawMethod = null;
                if (type != null)
                {
                    MethodBase[] methods;

                    if (methodName == type.Name || methodName == ".ctor")
                    {//���췽��
                        methods = type.GetConstructors(AllFlag);
                        methodName = ".ctor";
                    }
                    else
                    {
                        methods = type.GetMethods(AllFlag);
                    }

                    rawMethod = FindMethod(methods, methodName, hookMethod, assemblies);
                }
                if (rawMethod != null && rawMethod.IsGenericMethod)
                {
                    //���ͷ���ת��ʵ�ʷ���
                    rawMethod = ((MethodInfo)rawMethod).MakeGenericMethod(hookMethod.GetParameters().Select(o =>
                    {
                        var rt = o.ParameterType;
                        var attr = o.GetCustomAttribute<RememberTypeAttribute>();
                        if (attr != null && attr.TypeFullNameOrNull != null)
                        {
                            rt = TypeResolver(attr.TypeFullNameOrNull, assemblies);
                        }
                        return rt;
                    }).ToArray());
                }

                if (rawMethod == null)
                {
                    if (isInstall)
                    {

                    }
                    continue;
                }
                if (detour.Obj is IMethodHookWithSet)
                {
                    ((IMethodHookWithSet)detour.Obj).HookMethod(rawMethod);
                }

                var originalMethod = detour.OriginalMethod;
                var engine = DetourFactory.CreateDetourEngine();
                engine.Patch(rawMethod, hookMethod, originalMethod);

                /*Console.WriteLine("�ѽ�Ŀ�귽�� \"{0}, {1}\" �ĵ���ָ�� \"{2}, {3}\" Ori: \"{4}\".", rawMethod.ReflectedType.FullName, rawMethod
                    , hookMethod.ReflectedType.FullName, hookMethod
                    , originalMethod == null ? " (��)" : originalMethod.ToString());*/
            }
        }

        private static Type TypeResolver(string typeName, Assembly[] assemblies)
        {
            return Type.GetType(typeName, null, (a, b, c) =>
            {
                Type rt;
                if (a != null)
                {
                    rt = a.GetType(b);
                    if (rt != null)
                    {
                        return rt;
                    }
                }
                rt = Type.GetType(b);
                if (rt != null)
                {
                    return rt;
                }
                foreach (var asm in assemblies)
                {
                    rt = asm.GetType(b);
                    if (rt != null)
                    {
                        return rt;
                    }
                }
                return null;
            });
        }
        //����ƥ�亯��
        private static MethodBase FindMethod(MethodBase[] methods, string name, MethodBase like, Assembly[] assemblies)
        {
            var likeParams = like.GetParameters();
            foreach (var item in methods)
            {
                if (item.Name != name)
                {
                    continue;
                }

                var paramArr = item.GetParameters();
                var len = paramArr.Count();
                if (len != likeParams.Count())
                {
                    continue;
                }

                for (var i = 0; i < len; i++)
                {
                    var t1 = likeParams[i];
                    var t2 = paramArr[i];
                    //������ͬ ���� fullname��Ϊnull�ķ��Ͳ���
                    if (t1.ParameterType.FullName == t2.ParameterType.FullName)
                    {
                        continue;
                    }

                    //�ֶ����ֵ�����
                    var rmtype = t1.GetCustomAttribute<RememberTypeAttribute>();
                    if (rmtype != null)
                    {
                        //���Ͳ���
                        if (rmtype.IsGeneric && t2.ParameterType.FullName == null)
                        {
                            continue;
                        }
                        //����ʵ������
                        if (rmtype.TypeFullNameOrNull != null)
                        {
                            if (rmtype.TypeFullNameOrNull == t2.ParameterType.FullName)
                            {
                                continue;
                            }

                            var type = TypeResolver(rmtype.TypeFullNameOrNull, assemblies);
                            if (type == t2.ParameterType)
                            {
                                continue;
                            }
                        }
                    }
                    goto next;
                }
                return item;
            next:
                continue;
            }
            return null;
        }
    }
}
