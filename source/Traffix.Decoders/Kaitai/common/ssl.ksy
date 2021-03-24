meta:
  id: ssl_packet
  endian: be
seq:
  - id: length
    type: ssl_length
  - id: record
    type: ssl_record
    size: length.record
  - id: padding
    size: length.padding
types:
  ssl_length:
    seq:
      - id: len1
        type: u1
      - id: len2
        type: u1
      - id: len3
        type: u1
        if: has_padding
    instances:
      has_padding: 
        value: '(len1 & 0x80) == 0' 
      is_escape:
        value: 'len1 & 0x40'
      record: 
        value: '(((len1 & 0x7f) << 8) + len2)'
      padding:
        value: 'has_padding ? len3 : 0'
  ssl_record:
    seq:
      - id: message_type
        type: u1
        enum: ssl_message_type
      - id: message
        type:
          switch-on: message_type
          cases:
            'ssl_message_type::client_hello' : ssl_client_hello
            _ : ssl_encrypted_message
  ssl_client_hello:
    seq: 
      - id: version
        type: ssl_version
      - id: cipher_spec_length
        type: u2
      - id: session_id_length
        type: u2
      - id: challenge_length
        type: u2
      - id: cipher_specs
        type: ssl_cipher_list
        size: cipher_spec_length
      - id: session_id
        size: session_id_length
      - id: challenge
        size: challenge_length
  ssl_encrypted_message:
    seq: 
      - id: bytes
        size-eos: true  
  ssl_version:
    seq:
      - id: major
        type: u1
      - id: minor
        type: u1
  
  ssl_cipher_list:
    seq: 
     - id: entries
       type: ssl_cipher_spec
       repeat: eos
        
  ssl_cipher_spec:
    seq:
      - id: cipher_bytes
        size: 3
enums:
  ssl_message_type:
    0x0: error
    0x1: client_hello
    0x2: client_master_key
    0x3: client_finished
    0x4: server_hello
    0x5: server_verify
    0x6: server_finished
    0x7: request_certificate
    0x8: client_certificate