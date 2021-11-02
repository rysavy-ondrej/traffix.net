using MessagePack;
using System;

namespace Traffix.Processors
{
    /// <summary>
    /// The record produced by frame processor. It contains a fixed part and 
    /// processor specific data. 
    /// </summary>
    /// <typeparam name="TData">The data specific to flow processor.</typeparam>
    [MessagePackObject]
    public class FrameRecord<TData>
    {

        /// <summary>
        /// The label is mostly used for classification.
        /// </summary>
        [Key("FRAME_LABEL")]
        public RecordLabel Label;

        /// <summary>
        /// The frame record timestamp.
        /// </summary>
        [Key("TIME_STAMP")]
        public DateTime Timestamp;

        /// <summary>
        /// The frame record timestamp.
        /// </summary>
        [Key("FRAME_LENGTH")]
        public int FrameLength;

        /// <summary>
        /// The frame record data.
        /// </summary>
        [Key("DATA")]
        public TData Data;

        /// <summary>
        /// Creates a new <see cref="FrameRecord{TData}"/> by applying <paramref name="transform"/> function 
        /// to the current object.
        /// </summary>
        /// <typeparam name="TTarget">The type of target record.</typeparam>
        /// <param name="transform">The tranform function to apply.</param>
        /// <returns>A new <see cref="FrameRecord{TData}"/> object.</returns>
        public FrameRecord<TTarget> Transform<TTarget>(Func<TData, TTarget> transform)
        {
            return new FrameRecord<TTarget>
            {
                Label = this.Label,
                Timestamp = this.Timestamp,
                Data = transform(Data)
            };
        }

        public static Func<FrameRecord<TData>, FrameRecord<TTarget>> TransformTo<TTarget>(Func<TData, TTarget> transform)
        {
            return x => x.Transform(transform);
        }
    }
}