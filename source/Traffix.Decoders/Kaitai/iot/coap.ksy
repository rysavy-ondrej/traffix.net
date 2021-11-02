meta:
  id: coap_packet
  title: Constrainted Application Protocol (CoAP)
  endian: be
doc: |
  The Constrained Application Protocol (CoAP) is a specialized web transfer protocol for use with constrained 
  nodes and constrained networks in the Internet of Things. 
  The protocol is designed for machine-to-machine (M2M) applications such as smart energy and building automation.

  More information can be found at: http://coap.technology/
  and https://tools.ietf.org/html/rfc7252

seq:
  - id: version
    type: b2  
  - id: type
    type: b2 
    enum: coap_message_type
    
  - id: tkl
    type: b4 
  - id: code
    type: u1
    enum: coap_code
  - id: message_id
    type: u2
  - id: token
    size: tkl
  
  # options are optional :)  
  - id: options
    type: option
    repeat: until
    repeat-until: _.is_payload_marker or _io.eof
    if: _io.eof == false
    
  - id: body
    size-eos: true
    
types:
  option:
    doc: |
      Each option instance in a message specifies the Option Number of the
      defined CoAP option, the length of the Option Value, and the Option
      Value itself. Option nunber is expressed as delta.
      Both option length and delta values uses packing. Option is represented as 
      4 bits for regular values from 0-12. Values 13 and 14 informs that 
      option length is provided in extra bytes. The same holds for delta. 
    seq:
      - id: opt_delta
        type: b4
        
      - id: opt_len
        type: b4
      
      - id: opt_delta_extra_u1
        type: u1
        if: opt_delta == 13
        
      - id: opt_delta_extra_u2
        type: u2
        if: opt_delta == 14
      
      - id: opt_len_extra_u1
        type: u1
        if: opt_len == 13
        
      - id: opt_len_extra_u2
        type: u2
        if: opt_len == 14
        
      - id: value
        size: length
    instances:
      delta:
        value: 'opt_delta == 13 ? (opt_delta_extra_u1 + 13) : (opt_delta == 14 ? (opt_delta_extra_u2 + 269) : (opt_delta == 15 ? 0 : opt_delta))'
      length: 
        value: 'opt_len  == 13 ? (opt_len_extra_u1 + 13) : (opt_len == 14 ? (opt_len_extra_u2 + 269) : (opt_len == 15 ? 0 : opt_len))'
      is_payload_marker:
        value: 'opt_len == 15 and opt_delta == 15'

enums:
  coap_message_type:
    0x00: confirmable
    0x01: non_comfirmanble
    0x02: acknowledgement
    0x03: reset
  # COAP REQUEST AND RESPONSE CODES (https://tools.ietf.org/html/rfc7252#section-12.1)
  coap_code:
    0: empty
    1: get
    2: post
    3: put
    4: delete
    # 2.0x
    0x41: created         # 2.01
    0x42: deleted         # 2.02
    0x43: valid           # 2.03
    0x44: changed         # 2.04
    0x45: content         # 2.05
    # 4.0x
    0x80: bad_request
    0x81: unathorized
    0x82: bad_option
    0x83: forbidden
    0x84: not_found
    0x85: method_not_allowed
    0x86: not_acceptable
    0x8c: precondition_failed
    0x8d: request_entity_too_large
    0x8f: unsupported_content_format
    0xa0: internal_server_error
    0xa1: not_implemented
    0xa2: bad_gateway
    0xa3: service_unavailable
    0xa4: gateway_timeout
    0xa5: proxying_not_supported

  coap_options:
    1:  if_match
    3:  uri_host
    4:  etag
    5:  if_none_match
    7:  uri_port
    8:  location_path
    11: uri_path    
    12: content_format
    14: max_age
    15: uri_query
    17: accept
    20: location_query    
    35: proxy_uri
    39: proxy_scheme
    60: size1 