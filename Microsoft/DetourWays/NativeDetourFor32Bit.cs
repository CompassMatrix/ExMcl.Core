using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Threading;
using System.Linq.Expressions;
using System.IO;
using System.Diagnostics;
using Microsoft;

namespace DotNetDetour.DetourWays
{
    /// <summary>
    /// inline hook,ͨ���޸ĺ�����ǰ5�ֽ�ָ��Ϊjmp target_addrʵ��
    /// </summary>
    public unsafe class NativeDetourFor32Bit : IDetour
    {
        //protected byte[] originalInstrs = new byte[5];
        protected byte[] newInstrs = { 0xE9, 0x90, 0x90, 0x90, 0x90 }; //jmp target
        protected byte* rawMethodPtr;

        public NativeDetourFor32Bit()
        {
        }

        public virtual void Patch(MethodBase rawMethod/*Ҫhook��Ŀ�꺯��*/,
            MethodBase hookMethod/*�û�����ĺ��������Ե���ԭʼռλ������ʵ�ֶ�ԭ�����ĵ���*/,
            MethodBase originalMethod/*ԭʼռλ����*/)
        {
            //ȷ��jit����
            var typeHandles = rawMethod.DeclaringType.GetGenericArguments().Select(t => t.TypeHandle).ToArray();
            RuntimeHelpers.PrepareMethod(rawMethod.MethodHandle, typeHandles);

            rawMethodPtr = (byte*)rawMethod.MethodHandle.GetFunctionPointer().ToPointer();

            var hookMethodPtr = (byte*)hookMethod.MethodHandle.GetFunctionPointer().ToPointer();
            //������תָ�ʹ����Ե�ַ��������ת���û����庯��
            fixed (byte* newInstrPtr = newInstrs)
            {
                *(uint*)(newInstrPtr + 1) = (uint)hookMethodPtr - (uint)rawMethodPtr - 5;
            }


            //����ռλ�����ĵ���ָ��ԭ������ʵ�ֵ���ռλ����������ԭʼ�����Ĺ���
            if (originalMethod != null)
            {
                MakePlacholderMethodCallPointsToRawMethod(originalMethod);
            }


            //���ҽ���ԭ�����ĵ���ָ����תָ��Դ�ʵ�ֽ���ԭʼĿ�꺯���ĵ�����ת���û����庯��ִ�е�Ŀ��
            Patch();

            Debug.WriteLine("Patch: Point=" + rawMethod.MethodHandle.GetFunctionPointer().ToInt64() + " Method=" + rawMethod + " Type=" + rawMethod.ReflectedType.FullName);
        }

        protected virtual void Patch()
        {
            uint oldProtect;
            //ϵͳ����û��дȨ�ޣ���Ҫ�޸�ҳ����
            NativeAPI.VirtualProtect((IntPtr)rawMethodPtr, 5, Protection.PAGE_EXECUTE_READWRITE, out oldProtect);
            for (int i = 0; i < newInstrs.Length; i++)
            {
                *(rawMethodPtr + i) = newInstrs[i];
            }
        }

        /// <summary>
        /// ����originalMethod�ĵ���ָ��ԭ����
        /// </summary>
        /// <param name="originalMethod"></param>
        protected virtual void MakePlacholderMethodCallPointsToRawMethod(MethodBase originalMethod)
        {
            uint oldProtect;
            var needSize = LDasm.SizeofMin5Byte(rawMethodPtr);
            var total_length = (int)needSize + 5;
            byte[] code = new byte[total_length];
            IntPtr ptr = Marshal.AllocHGlobal(total_length);
            //code[0] = 0xcc;//������
            for (int i = 0; i < needSize; i++)
            {
                code[i] = rawMethodPtr[i];
            }
            code[needSize] = 0xE9;
            fixed (byte* p = &code[needSize + 1])
            {
                *((uint*)p) = (uint)rawMethodPtr - (uint)ptr - 5;
            }
            Marshal.Copy(code, 0, ptr, total_length);
            NativeAPI.VirtualProtect(ptr, (uint)total_length, Protection.PAGE_EXECUTE_READWRITE, out oldProtect);
            RuntimeHelpers.PrepareMethod(originalMethod.MethodHandle);
            *((uint*)originalMethod.MethodHandle.Value.ToPointer() + 2) = (uint)ptr;
        }
    }
}
