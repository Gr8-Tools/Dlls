namespace LinqToDbApi.Settings.Utils
{
    public struct NativeConnectionStringSettingsOptionsData
    {
        public string? Server { get; set; }
        public int? Port { get; set; }
        
        public string? DataBase { get; set; } 
        
        public string? UserId { get; set; }
        public string? Password { get; set; }
        
        public int? CommandTimeout { get; set; } 
        public int? ConnectionTimeout { get; set; }
        
        public int? Protocol { get; set; }
        
        public int? MinPoolSize { get; set; } 
        public int? MaxPoolSize{ get; set; }
        public int? ConnectionLifeTime { get; set; }

        public bool NotDefaultConnection => Server != null || Port != null;

        public bool WindowsSecuritySupported => DataBase != null;

        public bool UserExists => WindowsSecuritySupported && UserId != null && Password != null;

        public bool CommandTimeoutSupported => UserExists && CommandTimeout != null;
        
        public bool ConnectionTimeoutSupported => UserExists && ConnectionTimeout != null;
        
        public bool ProtocolConnectionsSupported => UserExists && Protocol != null;

        public bool PoolControllingSupported => ProtocolConnectionsSupported &&
            MinPoolSize != null && MaxPoolSize != null && ConnectionLifeTime != null;

    }
}