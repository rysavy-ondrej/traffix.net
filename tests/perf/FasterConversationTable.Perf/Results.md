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

|                      Method |              dataset |        Mean |       Error |      StdDev |         Min |         Max |      Median |
|---------------------------- |--------------------- |------------:|------------:|------------:|------------:|------------:|------------:|
|               ExportPackets | D:\Ca(...).pcap [61] |    127.3 ms |    64.28 ms |    16.69 ms |    115.4 ms |    154.2 ms |    117.5 ms |
|       ExportWindowedPackets | D:\Ca(...).pcap [61] |          NA |          NA |          NA |          NA |          NA |          NA |
|                 ExportFlows | D:\Ca(...).pcap [61] |    502.7 ms |   168.31 ms |    43.71 ms |    469.2 ms |    576.4 ms |    480.6 ms |
|         ExportWindowedFlows | D:\Ca(...).pcap [61] |    539.1 ms |   146.03 ms |    37.92 ms |    475.0 ms |    575.0 ms |    550.9 ms |
|         ExportConversations | D:\Ca(...).pcap [61] |    609.2 ms |   185.85 ms |    48.26 ms |    562.6 ms |    681.8 ms |    607.0 ms |
| ExportWindowedConversations | D:\Ca(...).pcap [61] |    563.2 ms |   129.50 ms |    33.63 ms |    520.3 ms |    598.6 ms |    551.8 ms |
|               ExportPackets | D:\Ca(...).pcap [61] |    615.4 ms |   162.48 ms |    42.19 ms |    579.2 ms |    676.3 ms |    597.6 ms |
|       ExportWindowedPackets | D:\Ca(...).pcap [61] |          NA |          NA |          NA |          NA |          NA |          NA |
|                 ExportFlows | D:\Ca(...).pcap [61] |  4,867.1 ms |   738.17 ms |   191.70 ms |  4,701.0 ms |  5,126.3 ms |  4,778.0 ms |
|         ExportWindowedFlows | D:\Ca(...).pcap [61] |  4,385.0 ms |   732.66 ms |   190.27 ms |  4,171.8 ms |  4,618.4 ms |  4,384.1 ms |
|         ExportConversations | D:\Ca(...).pcap [61] |  5,703.2 ms |   881.98 ms |   229.05 ms |  5,494.7 ms |  6,085.8 ms |  5,667.3 ms |
| ExportWindowedConversations | D:\Ca(...).pcap [61] |  5,041.2 ms |   466.58 ms |   121.17 ms |  4,947.1 ms |  5,242.9 ms |  5,018.4 ms |
|               ExportPackets | D:\Ca(...).pcap [61] |  1,007.6 ms |   280.63 ms |    72.88 ms |    920.3 ms |  1,099.8 ms |    994.9 ms |
|       ExportWindowedPackets | D:\Ca(...).pcap [61] |          NA |          NA |          NA |          NA |          NA |          NA |
|                 ExportFlows | D:\Ca(...).pcap [61] | 25,864.3 ms | 2,080.57 ms |   540.32 ms | 24,992.0 ms | 26,297.0 ms | 26,026.8 ms |
|         ExportWindowedFlows | D:\Ca(...).pcap [61] | 25,037.8 ms |   723.66 ms |   187.93 ms | 24,770.8 ms | 25,291.8 ms | 25,023.1 ms |
|         ExportConversations | D:\Ca(...).pcap [61] | 33,284.5 ms | 4,370.63 ms | 1,135.04 ms | 31,531.4 ms | 34,428.1 ms | 33,663.8 ms |
| ExportWindowedConversations | D:\Ca(...).pcap [61] | 31,834.9 ms | 1,094.93 ms |   284.35 ms | 31,440.5 ms | 32,193.2 ms | 31,800.5 ms |