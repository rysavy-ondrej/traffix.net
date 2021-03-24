namespace Traffix.Core.Flows
{
    /// <summary>
    /// Represents a flowdirection within a conversation.
    /// </summary>
    public enum FlowDirection
    {
        /// <summary>
        /// Forward flow consists of packets sent by the client to the server.
        /// </summary>
        Forward,
        /// <summary>
        /// Reverse flow consists of packets sent by the server to the client. 
        /// </summary>
        Reverse
    }
}
