meta:
  id: snmp
  file-extension: pcap
  endian: be
seq:
  - id: hdr
    type: asn_hdr  
  - id: version
    type: asn_obj  
  - id: community
    type: asn_obj
  - id: pdu_type
    type: asn_hdr
  - id: data
    type:
        switch-on: 'pdu_type.tag'
        cases:
          'asn_type_tag::snmp_pdu_get': get_request
          'asn_type_tag::snmp_pdu_getnext': get_next_request
          'asn_type_tag::snmp_pdu_response': response
          'asn_type_tag::snmp_pdu_set': set_request
          'asn_type_tag::snmp_pdu_trapv1': trap1
          'asn_type_tag::snmp_pdu_trapv2': trap2
    
types:
  asn_obj:
    seq:
    - id: hdr
      type: asn_hdr
    - id: body
      size: hdr.len.value
      type:
        switch-on: hdr.tag
        cases:
          'asn_type_tag::sequence_10': body_sequence
          'asn_type_tag::sequence_30': body_sequence
          'asn_type_tag::set': body_sequence
          'asn_type_tag::utf8string': body_utf8string
          'asn_type_tag::printable_string': body_printable_string
          'asn_type_tag::integer': body_integer
          'asn_type_tag::octet_string' : body_printable_string

  asn_hdr:
    seq:
      - id: tag
        type: u1 
        enum: asn_type_tag
      - id: len
        type: len_encoded
        
  error_status:
    seq:
      - id: hdr
        type: asn_hdr
      - id: val
        size: hdr.len.value
        type: body_integer
    instances:
      code:
        enum: snmp_error_status
        value: val.value
        
  get_request:
    seq: 
      - id: pdu
        type: pdu

  get_next_request:
    seq:
      - id: pdu
        type: pdu
   
  response:
    seq: 
      - id: pdu
        type: pdu

  set_request:
    seq: 
      - id: pdu
        type: pdu
        
  
  trap2:
    seq:
      - id: request_id
        type: pdu 
        
  trap1:
    seq:
      - id: items
        type: asn_obj
        repeat: eos
 
  pdu:
    seq:
      - id: request_id
        type: asn_obj
      - id: error_status
        type: error_status
      - id: error_index
        type: asn_obj
      - id: variable_bindings
        type: variable_bindings
        
  variable_bindings:
    seq:
      - id: seq_type_tag
        contents: [ 0x30 ]
        
      - id: len
        type: len_encoded
        
      - id: entries
        type: variable_binding
        repeat: eos
        
  variable_binding:
    seq:
      - id: seq_type_tag
        contents: [ 0x30 ]
        
      - id: len
        type: len_encoded
        
      - id: name
        type: asn_obj
        
      - id: value
        type: asn_obj
        
  len_encoded:
    seq:
      - id: b1
        type: u1
      - id: b2
        size: '(b1 < 128) ? 0 : b1 & 0x7f'
        type: body_integer
    instances:
      value:
        value: '(b1 < 128) ? b1 : b2.value'
  body_sequence:
    seq:
      - id: entries
        type: asn_obj
        repeat: eos
  body_utf8string:
    seq:
      - id: value
        type: str
        size-eos: true
        encoding: UTF-8
  body_printable_string:
    seq:
      - id: value
        type: str
        size-eos: true
        encoding: ASCII # actually a subset of ASCII
  body_integer:
    seq: 
      - id: bytes
        type: u1
        repeat: eos
    instances:
      value:
        value: >-
          bytes[bytes.size-1]
          + (bytes.size-1 >= 2 ? (bytes[(bytes.size-1) - 1] << 8) : 0)
          + (bytes.size-1 >= 3 ? (bytes[(bytes.size-1) - 2] << 16) : 0)
          + (bytes.size-1 >= 4 ? (bytes[(bytes.size-1) - 3] << 24) : 0)
          + (bytes.size-1 >= 5 ? (bytes[(bytes.size-1) - 4] << 32) : 0)
          + (bytes.size-1 >= 6 ? (bytes[(bytes.size-1) - 5] << 40) : 0)
          + (bytes.size-1 >= 7 ? (bytes[(bytes.size-1) - 6] << 48) : 0)
          + (bytes.size-1 >= 8 ? (bytes[(bytes.size-1) - 7] << 56) : 0)
        doc: Resulting value as normal integer
        
enums:
  asn_type_tag:
    0: end_of_content
    0x1: boolean
    0x2: integer
    0x3: bit_string
    0x4: octet_string
    0x5: null_value
    0x6: object_id
    0x7: object_descriptor
    0x8: external
    0x9: real
    0xa: enumerated
    0xb: embedded_pdv
    0xc: utf8string
    0xd: relative_oid
    0x10: sequence_10
    0x13: printable_string
    0x16: ia5string
    0x30: sequence_30
    0x31: set
    0xa0: snmp_pdu_get
    0xa1: snmp_pdu_getnext
    0xa2: snmp_pdu_response
    0xa3: snmp_pdu_set
    0xa4: snmp_pdu_trapv1
    0xa7: snmp_pdu_trapv2
  snmp_pdu_type:
    # SNMP PDU types:
    0: snmp_pdu_get
    1: snmp_pdu_getnext
    2: snmp_pdu_response
    3: snmp_pdu_set
    4: snmp_pdu_trapv1 
    7: snmp_pdu_trapv2
  snmp_error_status:
    0 : no_error
    1 : too_big
    2 : no_such_name
    3 : bad_value
    4 : read_only
    5 : gen_err
