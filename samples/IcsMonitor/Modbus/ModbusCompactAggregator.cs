using AutoMapper;

namespace IcsMonitor.Modbus
{
    internal class ModbusCompactAggregator : IModbusAggregator
    {
        private Mapper _mapper;

        public ModbusCompactAggregator()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ModbusFlowData, CompactModbusFlowData>());
            _mapper = new Mapper(config);
        }

        public object Apply(ModbusFlowData value)
        {
            return _mapper.Map<CompactModbusFlowData>(value);
        }
    }
}