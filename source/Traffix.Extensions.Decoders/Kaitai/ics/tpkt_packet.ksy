meta:
  id: tpkt_packet
  endian: be 
seq:
  - id: version
    type: u1
  - id: reserved
    type: u1
  - id: length
    type: u2
  - id: cotp
    type: cotp_packet
  - id: payload
    size: length - _io.pos
types:
  cotp_packet:
    seq:
    - id: length
      type: u1
    - id: pdu_type
      type: u1
      enum: cotp_type
    - id: options
      size: length - 1
      type: 
        switch-on: pdu_type
        cases:
          "cotp_type::data_transfer": cotp_data_transfer
          _: cotp_options
  
  cotp_options:
    seq:
    - id: bytes
      size-eos: true
      
  cotp_data_transfer:
    seq:
    - id: last_data
      type: b1
    - id: tpdu_number
      type: b7
    - id: data
      size-eos: true
    
enums:
  cotp_type:
    0xe0: connection_request
    0xd0: connection_confirm
    0x80: disconnect_request
    0xc0: diconnect_confirm
    0xf0: data_transfer
