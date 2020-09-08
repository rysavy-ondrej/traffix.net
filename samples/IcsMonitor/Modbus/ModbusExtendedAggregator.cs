using AutoMapper;
using System;

namespace IcsMonitor.Modbus
{
    class ModbusExtendedAggregator : IModbusAggregator
    {
        private Mapper _mapper;

        public ModbusExtendedAggregator()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ModbusFlowData, ExtendedModbusFlowData>());
            _mapper = new Mapper(config);
        }

        public object Apply(ModbusFlowData value)
        {
            return _mapper.Map<ExtendedModbusFlowData>(value);
        }
    }
}
