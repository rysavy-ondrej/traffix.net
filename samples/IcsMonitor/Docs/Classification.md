# Traffic classification

The exposrted IPFIX records for ICS traffic comprise a suitable the input for ML-based classification. 
[ML.NET](https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/) toolkit is used for creating classifiers.
This document provides reproducible description of experiments on designing and evaluating classifiers for Modbus IPFIX.  

## Auto ML
As the initial approach, the automate training capability of ML.NET was tested.
The `train.csv` dataset was prepared from `captures1/modbusQueryFlooding/eth2dump-modbusQueryFlooding30m-1h_1.pcap`
available from https://github.com/tjcruz-dei/ICS_PCAPS/releases/tag/MODBUSTCP%231:

```
.\IcsMonitor.exe Extract-ModbusFlows -inputFile eth2dump-modbusQueryFlooding-30m-1h_1.pcap -outformat csv -outFile train.csv
```
Before, we can execute the learning we need to annotate data. Fortunatelly, the attack is distighuished 
from the regular traffic in the dataset by source addresss:
* `172.27.224.251` represents a regular node 
* `172.27.224.50` represents an attacker
 
```
mlnet classification --dataset .\csv\train.csv --label-col=label --has-header=true --cache=off --ignore-cols rev_Start fwd_Start key_SourcePort key_SourceIpAddress key_DestinationPort key_DestinationIpAddress key_ProtocolType
```


Finally, to see how well the classifier works, we can try it on the testing dataset:
```
IcsMonitor.exe Extract-ModbusFlows -inputFile .\caps\eth2dump-modbusQueryFlooding-30m-12h_1.pcap -outformat csv -outFile .\csv\test.csv
```