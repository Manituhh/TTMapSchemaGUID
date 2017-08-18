/// Credits Pinvoke.net
/// Source: http://www.pinvoke.net/default.aspx/ntdsapi.DsMapSchemaGuids
/// Works with .NET version 2.x
using System;
using System.Runtime.InteropServices;


namespace TTMapSchemaGUID.Utility
{
    /// <summary>
    /// DsMap Class
    /// </summary>
    public class DSMap
    {
        MapParam mapParam;
        public DSMap (MapParam mapParam)
        {
            this.mapParam = mapParam;
        }
        /// <summary>
        /// DoMapSchema
        /// </summary>
        /// <returns>MapParam</returns>
        public MapParam DoMapSchema()
        {

            IntPtr phDS = new IntPtr(0);
            IntPtr guidMap = new IntPtr(0);
            uint result;

            Guid[] myGuids = { new Guid(mapParam.GuId) };

            //Testing IDs
            //Guid[] myGuids = {
            //    new Guid("{e0fa1e69-9b45-11d0-afdd-00c04fd930c9}"),
            //    new Guid("{771727b1-31b8-4cdf-ae62-4fe39fadf89e}"),
            //    new Guid("{d5eb2eb7-be4e-463b-a214-634a44d7392e}"),
            //    new Guid("{e0fa1e8c-9b45-11d0-afdd-00c04fd930c9}")
            //    };

            //Result of 0 is successful.
            result = DSBind(mapParam.DomainCon, mapParam.Domain, out phDS);

            if (result == 0)
            {
                result = DsMapSchemaGuids(phDS, (uint)myGuids.Length, myGuids, out guidMap);

                DS_SCHEMA_GUID_MAP[] guidMapResult = parseGuids(guidMap, myGuids.Length, true);
                if (guidMapResult.Length > 0)
                {
                    mapParam.DisplayName = guidMapResult[0].pName;
                    mapParam.GuidType = guidMapResult[0].guidType;
                }
            }
            else
            {
                throw new DSBindingException(string.Format("Verbindung mit Domain {0} auf den Domaincontroller {1} nicht möglich!", mapParam.Domain, mapParam.DomainCon));
            }

            return mapParam;
        }

        [DllImport("Ntdsapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint DsMapSchemaGuids(
            IntPtr hDs,
            uint cGuids,
            Guid[] rGuids,
            out IntPtr ppGuidMap);

        [DllImport("Ntdsapi.dll", SetLastError = true)]
        internal static extern void DsFreeSchemaGuidMap(
            IntPtr pGuidMap);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class GUID
        {
            internal uint Data1;
            internal int Data2;
            internal int Data3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            internal byte[] Data4 = new byte[8];
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DS_SCHEMA_GUID_MAP
        {
            internal GUID guid;
            internal uint guidType;
            internal string pName;
        }

        [DllImport("ntdsapi.dll", CharSet = CharSet.Auto)]
        static public extern uint DSBind(
          string DomainControllerName,
          string DnsDomainName,
          out IntPtr phDS);

        [DllImport("ntdsapi.dll", CharSet = CharSet.Auto)]
        static extern uint DsUnBind(
            IntPtr phDS);

        private static DS_SCHEMA_GUID_MAP[] parseGuids(IntPtr guidMap, int numGuids, bool freePointer)
        {
            int typeSize = Marshal.SizeOf(typeof(DS_SCHEMA_GUID_MAP));
            DS_SCHEMA_GUID_MAP[] schemaMap = new DS_SCHEMA_GUID_MAP[numGuids];
            IntPtr guidPointer = guidMap;


            for (int i = 0; i<numGuids; i++)
            {
                schemaMap[i] = (DS_SCHEMA_GUID_MAP) Marshal.PtrToStructure(

                new IntPtr  
                (
                    (long) guidPointer + i* typeSize
                ), 
                    typeof(DS_SCHEMA_GUID_MAP)
                );
            } 


            if (freePointer)
            {
                DsFreeSchemaGuidMap(guidPointer);
            }

            return schemaMap;
        }

    }
}
