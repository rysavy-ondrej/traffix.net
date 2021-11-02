meta:
  id: dlms_acse
  file-extension: acse_pdu
doc: >
    The ACSE APDUs are part of DLMS/COSEM communication during opening and 
    closing phase. ACSE APDUs are standardized by IEC 8650-1 standard or X.227
    The APDUs are encoded by BER for transmission. The user-information field 
    contains a COSEM pdu encoded by A-XDR.
    
    ACSE contains three pairs (request and response) of messages:
    * Application Association messages for creating association between cleint and server application.
    * Application Association Release Messages for finishing association. 
    * Application Association Abort Messages for aborting association in case of unexpected situation.

    References:
    
    * Distribution automation using distribution line carrier systems - Part 6: A-XDR encoding rule, IEC 61334-6:2000, 2006, Edition 1.
    * ITU-T Recommendation X.227: Data Networks and Open System Communications: Connection-oriented Protocol for the Association Control Service Element: Protocol Specification, ITU-T X.227, 1995.


    Limitations: 
    
    We parse only top level structure. Most of the fields uses BER encoding for their values.
    But there are some fields that used X-ADR encoding, e.g. User Information. 
seq:
  - id: pdu_type
    type: u1
    enum: acse_pdu_type
  - id: pdu
    type:
      switch-on: pdu_type
      cases:
        'acse_pdu_type::aarq': aarq_pdu
        'acse_pdu_type::aare': aare_pdu
        'acse_pdu_type::rlrq': rlrq_pdu
        'acse_pdu_type::rlre': rlre_pdu
        'acse_pdu_type::abrt': abrt_pdu
        'acse_pdu_type::adt': adt_pdu
types:
  aarq_pdu: 
    doc: | 
       Application Association Request PDU. It uses BER encoding 
       according to the the following ASN.1 schema:
       protocol-version [0] 
       application-context-name [1] 
       called-AP-title [2] 
       called-AE-qualifier [3] 
       called-AP-invocation-id [4] 
       called-AE-invocation-ide [5] 
       calling-AP-title [6] 
       calling-AE-quantifier [7] 
       calling-AP-invocation-id [8] 
       calling-AE-invocation-id [9] 
       sender-acse-requirements [10] 
       mechanism-name [11] 
       calling-authentication-value [12] 
       implementation-information [29] 
       user-information [30]
       
       Values are either application specific or context specific identifier, i.e., 
       they have 0xa or 0x8 prefix. 
       
       Only top-level information is parsed. To parse 
       user-information field use corresponding dlms parser.
    seq:
      - id: length
        type: ber_length
      - id: fields
        type: aarq_pdu_field
        repeat: eos
  aarq_pdu_field:
    seq:
      - id: tag
        type: u1
        enum: aarq_pdu_fields
      - id: length
        type: ber_length
      - id: value
        size: length.value
  aare_pdu:
    doc: | 
       Application Association Response PDU. It uses BER encoding 
       according to the the following ASN.1 schema:
        protocol-version                [0]
        application-context-name        [1]
        result                          [2]
        result-source-diagnostic        [3]
        responding-AP-title             [4]
        responding-AE-qualifier         [5] 
        responding-AP-invocation-id     [6]
        responding-AE-invocation-id     [7]
        responder-acse-requirements     [8]
        mechanism-name                  [9]
        responding-authentication-value [10] 
        application-context-name-list   [11] 
        implementation-information      [29] 
        user-information                [30] 
       
       Values are either application specific or context specific identifier, i.e., 
       they have 0xa or 0x8 prefix. 
       
       Only top-level information is parsed. To parse 
       user-information field use corresponding dlms parser.
    seq:
      - id: length
        type: ber_length
      - id: fields
        type: aare_pdu_field
        repeat: eos
  aare_pdu_field:
    seq:
      - id: tag
        type: u1
        enum: aare_pdu_fields
      - id: length
        type: ber_length
      - id: value
        size: length.value
  rlrq_pdu:
    {}
  rlre_pdu:
    {}
  abrt_pdu:
    {}
  adt_pdu:
    {}

  ber_length:
    seq:
      - id: b1
        type: u1
      - id: int2
        type: u2be
        if: b1 == 0x82
    instances:
      value:
        value: '(b1 & 0x80 == 0) ? b1 : int2' 
enums:
  acse_pdu_type:
    96: aarq
    97: aare
    98: rlrq
    99: rlre
    100: abrt
    101: adt
  aarq_pdu_fields:
    0xa0: protocol_version
    0xa1: application_context_name
    0xa2: called_ap_title
    0xa3: called_ae_quantifier
    0xa4: called_ap_invocation_id
    0xa5: called_ae_invocation_ide
    0xa6: calling_ap_title 
    0xa7: calling_ae_quantifier  
    0xa8: calling_ap_invocation_id 
    0xa9: calling_ae_invocation_id 
    0x8a: sender_acse_requirements  
    0x8b: mechanism_name 
    0xac: calling_authentication_value  
    0xbd: implementation_information 
    0xbe: user_information
  aare_pdu_fields: 
    0xa0: protocol_version
    0xa1: application_context_name
    0xa2: result
    0xa3: result_source_diagnostic
    0xa4: responding_ap_title
    0xa5: responding_ae_qualifier
    0xa6: responding_ap_invocation_id
    0xa7: responding_ae_invocation_id 
    0x88: responding_acse_requirements
    0x89: mechanism_name 
    0xaa: responding_authentication_value 
    0x8b: application_context_name_list
    0xbd: implementation_information 
    0xbe: user_information 
    