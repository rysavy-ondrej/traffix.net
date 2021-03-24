meta:
  id: s7comm_packet
  endian: be
seq:
  - id: protcol_id
    type: u1
  - id: message_type
    type: u1
    enum: s7_message_type
  - id: reserved
    type: u2
  - id: pdu_reference
    type: u2
  - id: parameters_length
    type: u2
  - id: data_length
    type: u2
  - id: error
    type: response_error
    if: "message_type == s7_message_type::ack_data"
  - id: message
    size: "parameters_length + data_length"
    type: 
      switch-on: message_type
      cases:
        "s7_message_type::job_request": job_request_message
        "s7_message_type::ack_data" : ack_data_message
        _ : message_other 
      
types:
  message_other:
    seq: 
    - id: parameters
      size: "_root.parameters_length"
    - id: data
      size: "_root.data_length"
  response_error:
    seq:
    - id: error_class
      type: u1
    - id: error_code
      type: u1
  job_request_message:
    seq:
    - id: function_code
      type: u1
      enum: s7_function_code
    - id: function
      type:
        switch-on: function_code
        cases:
          "s7_function_code::setup_communication": job_setup_communication
          "s7_function_code::read_variable" : job_read_variable
          "s7_function_code::write_variable" : job_write_variable
          _ : job_other
  ack_data_message:
    seq:
    - id: function_code
      type: u1
      enum: s7_function_code
    - id: function
      type:
        switch-on: function_code
        cases:
          's7_function_code::setup_communication': job_setup_communication      
          's7_function_code::read_variable' : ack_data_read_variable
          's7_function_code::write_variable' : ack_data_write_variable
          _ : ack_data_other
          
  job_other:
    seq: 
    - id: parameters
      size: "_root.parameters_length-1"
    - id: data
      size: "_root.data_length"
  
  job_setup_communication:
    seq:
    - id: reserved
      type: u1
    - id: max_amq_caller
      type: u2
    - id: max_amq_callee
      type: u2
    - id: max_pdu_length
      type: u2
      
  job_read_variable:
    seq:
    - id: item_count
      type: u1
    - id: items
      type: parameter_item
      repeat: expr
      repeat-expr: item_count
  ack_data_read_variable:
    seq:
    - id: item_count
      type: u1
    - id: data
      type: data_item(item_count-_index-1)   # workaround for: data_item(item_count-(_index+1)) because of syntax limitation
      repeat: expr
      repeat-expr: item_count 
      
  job_write_variable:
    seq:
    - id: item_count
      type: u1
    - id: items
      type: parameter_item
      repeat: expr
      repeat-expr: item_count
    - id: data
      type: data_item(item_count-_index-1)
      repeat: expr
      repeat-expr: item_count
      
  ack_data_write_variable:
    seq:
    - id: item_count
      type: u1
    - id: items
      type: u1
      enum: return_code
      repeat: expr
      repeat-expr: item_count      
  
  ack_data_other:
    seq: 
    - id: parameters
      size: "_root.parameters_length-1"
    - id: data
      size: "_root.data_length"
      
  parameter_item:
    seq:
    - id: parameter_type
      type: u1
    - id: parameter_length
      type: u1
    - id: syntax_id
      type: u1
    - id: transport_size
      type: u1
      enum: transport_size_type
    - id: length
      type: u2
    - id: db_number
      type: u2
    - id: area
      type: u1
      enum: area_code
    - id: address
      size: 3
      
  data_item:
    params:
    - id: remaining
      type: s4
    seq:
    - id: return_code
      type: u1
      enum: return_code
    - id: data_type
      type: u1
    - id: transport_size
      type: u2
    - id: data
      size: data_length
    - id: fill_byte
      size: 1
      if: "(data_length % 2) == 1 and remaining > 0"
    instances:
      data_length:
        value: "(data_type == 3 or data_type == 4 or data_type == 5) ? ((transport_size + 7)/8 ) : (transport_size)"
        doc: "If data type is 3,4,5 then length gives number of bits. As #3 can represent unaligned bits we need to round it to to bytes." 
   
enums:
  s7_message_type:
    0x01: job_request
    0x02: ack
    0x03: ack_data
    0x07: user_data
    0x08: server_control
  s7_function_code:
    0xF0: setup_communication
    0x04: read_variable
    0x05: write_variable
    0x1A: request_download
    0x1B: download_block
    0x1C: download_ended
    0x1D: start_upload
    0x1E: upload  
    0x1F: end_upload
    0x28: plc_control
    0x29: plc_stop
  transport_size_type:
    0: nul
    2: byte
    5: integer
  return_code:
    0x00: reserved
    0x01: data_hw_fault
    0x03: data_access_fault
    0x05: data_out_of_range
    0x06: data_type_nor_supported
    0x07: data_size_mismatch
    0x0a: data_not_available
    0xff: success
  error_class:
    0x0: no_error
    0x81: apprel
    0x82: objdef
    0x83: resource
    0x84: service
    0x85: supplies
    0x87: access

  area_code:
    0x83: flags
    0x84: data_blocks
  