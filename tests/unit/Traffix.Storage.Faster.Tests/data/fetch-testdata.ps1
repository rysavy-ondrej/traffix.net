if (!(Test-Path "testbed-16.pcap"))
{
    Write-Progress -Activity "Fetching 'testbed-16.pcap'"
    Start-BitsTransfer -Source "http://www.fit.vutbr.cz/~rysavy/traffix.net/captures/testbed-16.pcap" -Destination ".\testbed-16.pcap"
}

if (!(Test-Path "testbed-32.pcap"))
{
    Write-Progress -Activity "Fetching 'testbed-32.pcap'"
    Start-BitsTransfer -Source "http://www.fit.vutbr.cz/~rysavy/traffix.net/captures/testbed-32.pcap" -Destination ".\testbed-32.pcap"
}

if (!(Test-Path "testbed-64.pcap"))
{
    Write-Progress -Activity "Fetching 'testbed-64.pcap'"
    Start-BitsTransfer -Source "http://www.fit.vutbr.cz/~rysavy/traffix.net/captures/testbed-64.pcap" -Destination ".\testbed-64.pcap"
}

if (!(Test-Path "testbed-128.pcap"))
{
    Write-Progress -Activity "Fetching 'testbed-128.pcap'"
    Start-BitsTransfer -Source "http://www.fit.vutbr.cz/~rysavy/traffix.net/captures/testbed-128.pcap" -Destination ".\testbed-128.pcap"
}