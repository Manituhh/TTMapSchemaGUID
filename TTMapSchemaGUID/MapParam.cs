namespace TTMapSchemaGUID
{
    /// <summary>
    /// Class MapParam used by <see cref="MapSchemaGUID"/> to store Input/Output Parameters
    /// </summary>
    public class MapParam
    {
        public string DomainCon { get; set; }
        public string Domain { get; set; }
        public string GuId { get; set; }
        public string DisplayName { get; set; }
        public uint GuidType { get; set; }
    }
}
