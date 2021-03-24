meta:
  id: tls_packet
  endian: be
  license: MIT
seq:
  - id: content_type
    type: u1
    enum: tls_content_type
  
  - id: version
    type: u2
    
  - id: length
    type: u2

  - id: record
    size: length
    type:
      switch-on: content_type
      cases:
        'tls_content_type::change_cipher_spec': tls_change_cipher_spec
        'tls_content_type::alert': tls_encrypted_message
        'tls_content_type::handshake': tls_handshake_protocol
        'tls_content_type::application_data': tls_application_data
        _ : tls_encrypted_message

types:  
  tls_finished: 
    seq:
      - id: handshake_msg_type
        contents: [0x16]  
      - id: version
        type: u2      
      - id: length 
        type: u2
      - id: finished_bytes
        size: length 

  tls_change_cipher_spec:
    seq:
      - id: change_message
        size-eos: true

  tls_encrypted_message:
    seq:
      - id: encrypted_message
        size-eos: true

  tls_application_data:
    seq:
      - id: body
        size-eos: true
  tls_handshake_protocol:
    seq:
      - id: handshake_messages
        type: tls_handshake
        repeat: eos
  tls_handshake:
    seq:
      - id: msg_type
        type: u1
        enum: tls_handshake_type
      - id: length 
        type: tls_length
      - id: body
        size: length.value
        type:
          switch-on: msg_type
          cases:
            'tls_handshake_type::hello_request' : tls_hello_request
            'tls_handshake_type::client_hello': tls_client_hello
            'tls_handshake_type::server_hello': tls_server_hello
            'tls_handshake_type::certificate': tls_certificate
            'tls_handshake_type::server_key_exchange': tls_server_key_exchange
            'tls_handshake_type::certificate_request': tls_certificate_request
            'tls_handshake_type::server_hello_done': tls_server_hello_done
            'tls_handshake_type::certificate_verify': tls_certificate_verify
            'tls_handshake_type::client_key_exchange': tls_client_key_exchange
            
  tls_length:
    seq:
      - id: hlen
        type: u1
      - id: llen
        type: u2
    instances:
      value: 
        value: (llen + (hlen << 16))
    
  tls_hello_request:
    seq:
      - id: empty
        size: 0

  tls_server_hello_done:
    seq:
      - id: empty
        size: 0


  tls_certificate_verify:
    seq:
      - id: signed_handshake_message
        size-eos: true        
  
  tls_certificate_request:
    seq:
      - id: request
        size-eos: true   
  
  tls_server_key_exchange:
    seq:
      - id: server_key_exchange
        size-eos: true      
                
  tls_certificate:
    seq:
      - id: cert_length
        type: tls_length
      
      - id: certificates
        size: cert_length.value
        type: certificate
        repeat: eos
        
  certificate:
    seq:
      - id: cert_length
        type: tls_length
        
      - id: body
        size: cert_length.value
      
  tls_server_hello: 
     seq:
      - id: version
        type: u2
    
      - id: random
        type: random
    
      - id: session_id
        type: session_id
    
      - id: cipher_suite
        type: cipher_suite
    
      - id: compression_method
        type: u1
        enum: compression_methods
    
      - id: extensions
        type: tls_extensions
        if: _io.size > _io.pos
      
  tls_client_hello:
    seq:
      - id: version
        type: u2
    
      - id: random
        type: random
    
      - id: session_id
        type: session_id
    
      - id: cipher_suites
        type: cipher_suites
    
      - id: compression_methods_length
        type: u1
      - id: compression_methods
        type: u1
        enum: compression_methods
        repeat: expr
        repeat-expr: compression_methods_length
        
      - id: extensions
        type: tls_extensions
        if: _io.size > _io.pos
                    
  tls_client_key_exchange: 
    seq:
      - id: exchange_keys
        size-eos: true
        
  tls_pre_master_secret:
    seq:
      - id: secret_length
        type: u2
      
      - id: secret
        size: secret_length
      
  random:
    seq:
      - id: random_time
        size: 4

      - id: random_bytes
        size: 28

  session_id:
    seq:
      - id: len
        type: u1

      - id: sid
        size: len

  cipher_suites:
    seq:
      - id: length
        type: u2
      - id: items
        type: u2
        repeat: expr
        repeat-expr: length/2

  cipher_suite:
    seq:
      - id: cipher_id
        type: u2
          
  tls_extensions:
    seq: 
      - id: extensions_length
        type: u2  
      - id: extensions_list
        type: tls_extensions_list
        size: extensions_length
  tls_extensions_list:
    seq:
      - id: items
        type: tls_extension
        repeat: eos    
  tls_extension:
    seq:
      - id: type
        type: u2
      - id: length
        type: u2
      - id: body
        size: length

  sni:
    seq:
      - id: list_length
        type: u2

      - id: server_names
        type: server_name
        repeat: eos

  server_name:
    seq:
      - id: name_type
        type: u1

      - id: length
        type: u2

      - id: host_name
        size: length

  alpn:
    seq:
      - id: ext_len
        type: u2

      - id: alpn_protocols
        type: protocol
        repeat: eos

  protocol:
    seq:
      - id: strlen
        type: u1

      - id: name
        size: strlen
        
enums:
  tls_content_type:
    20: change_cipher_spec            # 0x14
    21: alert                         # 0x15
    22: handshake                     # 0x16
    23: application_data              # 0x17
    
  tls_handshake_type:
    0: hello_request
    1: client_hello
    2: server_hello
    4: new_session_ticket
    11: certificate
    12: server_key_exchange
    13: certificate_request
    14: server_hello_done
    15: certificate_verify
    16: client_key_exchange
    20: finished
    24: key_update
  compression_methods:
    0: null_compression
    1: deflate
    