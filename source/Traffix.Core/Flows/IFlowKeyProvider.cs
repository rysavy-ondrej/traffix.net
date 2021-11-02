namespace Traffix.Core.Flows
{
    /// <summary>
    /// Defines interface for key providers. The key provider returns 
    /// <see cref="FlowKey"/> for the packet.  
    /// </summary>
    public interface IFlowKeyProvider<TFlowKey, TPacket>
    {
        /// <summary>
        /// Gets the flow key for the given packet.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        TFlowKey GetKey(TPacket packet);
        /// <summary>
        /// Gets flow key hash for the given packet.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        int GetKeyHash(TPacket packet);
    }
}
