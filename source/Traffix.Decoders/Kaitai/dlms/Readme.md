# DLMS Parser Infrastructure

This folder contains a collection of Kaitai DLMS parsers:

* `dlms_hdlc.ksy` - parser fo LLC/HDLC communication profile, this parser can be used to decode DLMS coomunication encapsulated in LLC/HDLC communication
* `dlms_acse.ksy` - parsers for ACSE packets
* `dlms_apdu.ksy` - parsers for xDLMS application PDU
* `dlms_action_request.ksy`
* `dlms_action_response.ksy`
* `dlms_event_notification.ksy`
* `dlms_get_request.ksy`
* `dlms_get_response.ksy`
* `dlms_set_request.ksy`
* `dlms_set_response.ksy`
* `dlms_struct.ksy` - parsers for various common DLMS structures and objects
* `dlms_data.ksy` - parser for DLMS data type 

Both `dlms_hdlc.ksy` and `dlms_acse.ksy` can be used in Kaitai IDE. `dlms_apdu.ksy` imports other parsers and it seams that it cannot be used in IDE because of 
some problem with type referencing. However, the generated parsers (at least for C#) works. For some test data see Netdx/Test/Dataset/dlms/raw folder.

To test `dlms_hdlc.ksy` use: 

* `hdlc_01.raw`
* `hdlc_02.raw`
* `hdlc_03.raw` - this is i-frame but does not contain llc header.
* `dlms_get_request.raw` - example of get request command including hdlc/llc layer.

To test `dlms_acse.ksy` use the following files:
* `aarq_noath.raw` - example of AARQ APDU without the ACSE security
* `aarq_auth_l.raw` -  example of AARQ APDU using low level authentication
* `aarq_auth_h.raw` -  example of AARQ APDU using high level authentication
* `aare_noauth.raw` - example of AARE APDU not using security or using low level security
* `aare_auth_h.raw` example of AARE APDU using high level security
* `aare_fail1.raw` - example of AARE APDU case of failure 1
* `aare_fail2.raw` - example of AARE APDU case of failure 2

