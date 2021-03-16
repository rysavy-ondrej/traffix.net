# BenchmarkFaster

|                      Method |              dataset |         Mean |       Error |    StdDev |          Min |          Max |       Median |
|---------------------------- |--------------------- |-------------:|------------:|----------:|-------------:|-------------:|-------------:|
|                   LoadTable | D:\Ca(...).pcap [61] |     663.6 ms |    65.33 ms |  16.97 ms |     637.6 ms |     678.5 ms |     667.5 ms |
|               ExportPackets | D:\Ca(...).pcap [61] |     409.4 ms |   178.69 ms |  46.41 ms |     364.3 ms |     463.7 ms |     390.9 ms |
|         ExportConversations | D:\Ca(...).pcap [61] |     467.2 ms |   125.28 ms |  32.53 ms |     426.9 ms |     516.1 ms |     461.6 ms |
|  GroupWindowedConversations | D:\Ca(...).pcap [61] |     369.8 ms |   124.05 ms |  32.22 ms |     318.0 ms |     397.5 ms |     383.8 ms |
| ExportWindowedConversations | D:\Ca(...).pcap [61] |  15,641.1 ms |   794.58 ms | 206.35 ms |  15,386.5 ms |  15,844.6 ms |  15,672.0 ms |
|                   LoadTable | D:\Ca(...).pcap [61] |   2,726.2 ms |   206.23 ms |  53.56 ms |   2,655.6 ms |   2,783.3 ms |   2,743.5 ms |
|               ExportPackets | D:\Ca(...).pcap [61] |   1,462.1 ms |   122.83 ms |  31.90 ms |   1,430.6 ms |   1,501.5 ms |   1,459.3 ms |
|         ExportConversations | D:\Ca(...).pcap [61] |   2,098.2 ms |   299.61 ms |  77.81 ms |   2,038.8 ms |   2,230.6 ms |   2,074.7 ms |
|  GroupWindowedConversations | D:\Ca(...).pcap [61] |   1,154.9 ms |    89.87 ms |  23.34 ms |   1,127.2 ms |   1,187.7 ms |   1,153.1 ms |
| ExportWindowedConversations | D:\Ca(...).pcap [61] | 208,204.0 ms | 2,374.73 ms | 616.71 ms | 207,138.3 ms | 208,732.8 ms | 208,393.0 ms |
|                   LoadTable | D:\Ca(...).pcap [61] |   5,574.4 ms |   510.66 ms | 132.62 ms |   5,340.4 ms |   5,653.5 ms |   5,630.8 ms |
|               ExportPackets | D:\Ca(...).pcap [61] |   2,135.9 ms |   211.80 ms |  55.00 ms |   2,062.7 ms |   2,208.3 ms |   2,143.6 ms |
|         ExportConversations | D:\Ca(...).pcap [61] |   2,834.8 ms |    68.66 ms |  17.83 ms |   2,817.3 ms |   2,856.6 ms |   2,826.2 ms |
|  GroupWindowedConversations | D:\Ca(...).pcap [61] |   1,878.4 ms |    18.26 ms |   4.74 ms |   1,872.3 ms |   1,883.4 ms |   1,879.4 ms |
| ExportWindowedConversations | D:\Ca(...).pcap [61] |  76,113.8 ms | 3,141.90 ms | 815.94 ms |  75,331.5 ms |  77,309.8 ms |  76,038.5 ms |


# BenchmarkObservable

|                      Method |              dataset |        Mean | Error |    StdDev |         Min |         Max |      Median |
|---------------------------- |--------------------- |------------:|------:|----------:|------------:|------------:|------------:|
|               ExportPackets | D:\Ca(...).pcap [61] |    143.3 ms |    NA |   2.69 ms |    141.3 ms |    145.2 ms |    143.3 ms |
|    ExportPacketsWithFlowKey | D:\Ca(...).pcap [61] |    242.9 ms |    NA |  41.53 ms |    213.5 ms |    272.2 ms |    242.9 ms |
|       ExportWindowedPackets | D:\Ca(...).pcap [61] |    183.8 ms |    NA |  53.24 ms |    146.2 ms |    221.4 ms |    183.8 ms |
|                 ExportFlows | D:\Ca(...).pcap [61] |    558.4 ms |    NA |  68.28 ms |    510.1 ms |    606.7 ms |    558.4 ms |
|         ExportWindowedFlows | D:\Ca(...).pcap [61] |    551.3 ms |    NA |  16.06 ms |    539.9 ms |    562.7 ms |    551.3 ms |
|         ExportConversations | D:\Ca(...).pcap [61] |    615.0 ms |    NA |  13.29 ms |    605.6 ms |    624.4 ms |    615.0 ms |
| ExportWindowedConversations | D:\Ca(...).pcap [61] |    533.6 ms |    NA |  30.43 ms |    512.1 ms |    555.2 ms |    533.6 ms |
|               ExportPackets | D:\Ca(...).pcap [61] |    660.0 ms |    NA |   2.97 ms |    657.9 ms |    662.1 ms |    660.0 ms |
|    ExportPacketsWithFlowKey | D:\Ca(...).pcap [61] |  1,082.1 ms |    NA |  18.05 ms |  1,069.3 ms |  1,094.9 ms |  1,082.1 ms |
|       ExportWindowedPackets | D:\Ca(...).pcap [61] |    622.4 ms |    NA |  19.43 ms |    608.6 ms |    636.1 ms |    622.4 ms |
|                 ExportFlows | D:\Ca(...).pcap [61] |  4,587.1 ms |    NA | 175.35 ms |  4,463.1 ms |  4,711.0 ms |  4,587.1 ms |
|         ExportWindowedFlows | D:\Ca(...).pcap [61] |  4,450.2 ms |    NA |  82.21 ms |  4,392.0 ms |  4,508.3 ms |  4,450.2 ms |
|         ExportConversations | D:\Ca(...).pcap [61] |  5,788.8 ms |    NA | 123.21 ms |  5,701.7 ms |  5,875.9 ms |  5,788.8 ms |
| ExportWindowedConversations | D:\Ca(...).pcap [61] |  4,508.8 ms |    NA | 117.00 ms |  4,426.0 ms |  4,591.5 ms |  4,508.8 ms |
|               ExportPackets | D:\Ca(...).pcap [61] |  1,040.8 ms |    NA |  13.41 ms |  1,031.3 ms |  1,050.2 ms |  1,040.8 ms |
|    ExportPacketsWithFlowKey | D:\Ca(...).pcap [61] |  1,926.5 ms |    NA |  70.64 ms |  1,876.6 ms |  1,976.5 ms |  1,926.5 ms |
|       ExportWindowedPackets | D:\Ca(...).pcap [61] |  1,049.5 ms |    NA |  10.45 ms |  1,042.1 ms |  1,056.9 ms |  1,049.5 ms |
|                 ExportFlows | D:\Ca(...).pcap [61] | 25,297.2 ms |    NA | 126.03 ms | 25,208.1 ms | 25,386.3 ms | 25,297.2 ms |
|         ExportWindowedFlows | D:\Ca(...).pcap [61] | 24,923.3 ms |    NA | 269.42 ms | 24,732.8 ms | 25,113.8 ms | 24,923.3 ms |
|         ExportConversations | D:\Ca(...).pcap [61] | 31,440.4 ms |    NA | 893.27 ms | 30,808.8 ms | 32,072.1 ms | 31,440.4 ms |
| ExportWindowedConversations | D:\Ca(...).pcap [61] | 32,224.7 ms |    NA | 845.23 ms | 31,627.0 ms | 32,822.3 ms | 32,224.7 ms |rtWindowedConversations | D:\Ca(...).pcap [61] | 31,834.9 ms | 1,094.93 ms |   284.35 ms | 31,440.5 ms | 32,193.2 ms | 31,800.5 ms |