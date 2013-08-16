using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// auth request
    /// </summary>
    public sealed class AuthRequest : IRecord
    {
        /// <summary>
        /// type
        /// </summary>
        public readonly int Type;
        /// <summary>
        /// scheme
        /// </summary>
        public readonly string Scheme;
        /// <summary>
        /// auth data
        /// </summary>
        public readonly byte[] Auth;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scheme"></param>
        /// <param name="auth"></param>
        public AuthRequest(int type, string scheme, byte[] auth)
        {
            this.Type = type;
            this.Scheme = scheme;
            this.Auth = auth;
        }

        /// <summary>
        /// write
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            formatter.Write(this.Type);
            formatter.Write(this.Scheme);
            formatter.Write(this.Auth);
        }
        /// <summary>
        /// read
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            throw new NotImplementedException();
        }
    }
}