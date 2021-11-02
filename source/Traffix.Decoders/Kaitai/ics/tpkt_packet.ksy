meta:
  id: tpkt_packet
  endian: be 
seq:
  - id: tptk
    type: tptk_header
  - id: cotp
    type: cotp_header
  - id: opts
    type: cotp_options(cotp.pdu_type)
    size: "cotp.length - 1"
  - id: payload
    size: "tptk.length - _io.pos"
types:
  tptk_header:
    seq:
    - id: version
      type: u1
    - id: reserved
      type: u1
    - id: length
      type: u2    
  cotp_header:
    seq:
    - id: length
      type: u1
    - id: pdu_type
      type: u1
  cotp_options:
    params:
      - id: pdu_type
        type: u1
    seq:
    - id: options
      size-eos: true
      type: 
        switch-on: pdu_type
        cases:
          0xf0: cotp_data_transfer_options
          _: cotp_other_options
  
  cotp_data_transfer_options:
    seq:
    - id: last_data
      type: b1
    - id: tpdu_number
      type: b7
    - id: data
      size-eos: true
      
  cotp_other_options:
    seq:
    - id: bytes
      size-eos: true  
enums:
  cotp_type:
    0xe0: connection_request
    0xd0: connection_confirm
    0x80: disconnect_request
    0xc0: diconnect_confirm
    0xf0: data_transfer
