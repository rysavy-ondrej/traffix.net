meta:
  id: modbus_request_packet
  title: Modbus protocol parser for Query messages
  file-extension: raw
  endian: be
doc: |
  This is a simplified MODBUS parser. It only parses the header and identifies the function code of the message.
  A dedicated header is used on TCP/IP to identify the MODBUS Application Data Unit. It is called the MBAP header (MODBUS Application Protocol header).
  The problem with modbus messages is that there is not indication if the message is 
  Query or Response within the message. Parser for Query differs to that for Response. 
  We thus need to know whether the message is Query or Response before we start parsing.
  
doc-ref: 'http://www.modbus.org/docs/Modbus_Application_Protocol_V1_1b.pdf'
seq:
  - id: header
    type: mbap_header
  - id: function
    type: u1
    enum: modbus_function_code
  - id: data
    size: header.length - 2
    type:
      switch-on: function
      cases:
        'modbus_function_code::read_coil_status': read_coil_status_function
        'modbus_function_code::read_input_status': read_input_status_function
        'modbus_function_code::read_holding_register': read_holding_registers_function
        'modbus_function_code::read_input_registers': read_input_registers_function
        'modbus_function_code::write_single_coil': write_single_coil_function
        'modbus_function_code::write_single_register': write_single_register_function
        'modbus_function_code::read_input_registers': read_input_registers_function
        'modbus_function_code::write_multiple_coils': write_multiple_coils_function
        'modbus_function_code::write_multiple_registers': write_multiple_registers_function
        'modbus_function_code::read_file_record': read_file_record_function
        'modbus_function_code::write_file_record': write_file_record_function
        'modbus_function_code::mask_write_register': mask_write_register_function
        'modbus_function_code::read_write_multiuple_registers': read_write_multiuple_registers_function
        'modbus_function_code::read_fifo_queue': read_fifo_queue_function
        'modbus_function_code::read_device_identification': read_device_identification_function

types:
  mbap_header:
    seq:
    - id: transation_id
      type: u2
    - id: protocol_id
      type: u2
    - id: length
      type: u2
    - id: unit_identifier
      type: u1
      
  read_coil_status_function:
    seq:
    - id: starting_address
      type: u2
    - id: quantity_of_coils
      type: u2  
  read_input_status_function:
    seq:
    - id: starting_address
      type: u2
    - id: quantity_of_inputs
      type: u2 
  read_holding_registers_function:
    seq:
    - id: starting_address
      type: u2
    - id: quantity_of_inputs
      type: u2 
  read_input_registers_function:
    seq:
    - id: starting_address
      type: u2
    - id: quantity_of_inputs
      type: u2 
  write_single_coil_function:
    seq:
    - id: output_address
      type: u2
    - id: output_value
      type: u2  
  write_single_register_function:
    seq:
    - id: register_address
      type: u2
    - id: register_value
      type: u2    
  write_multiple_coils_function:
    seq:
    - id: starting_address
      type: u2
    - id: quantity_of_outputs
      type: u2   
    - id: byte_count
      type: u2  
    - id: output_values
      size: byte_count  
  
  write_multiple_registers_function:
    seq:
    - id: starting_address
      type: u2
    - id: quantity_of_registers
      type: u2   
    - id: byte_count
      type: u2  
    - id: register_values
      size: byte_count 

  read_file_record_function:
    seq:
    - id: byte_count
      type: u1
    - id: sub_requests
      size: byte_count
      type: read_file_record_requests
      repeat: eos
  read_file_record_requests:
    seq:
    - id: reference_type
      type: u1
    - id: file_number
      type: u2
    - id: record_number
      type: u2
    - id: record_length
      type: u2

  write_file_record_function:
    seq:
    - id: byte_count
      type: u1
    - id: sub_requests
      size: byte_count
      type: write_file_record_requests
      repeat: eos
  write_file_record_requests:
    seq:
    - id: reference_type
      type: u1
    - id: file_number
      type: u2
    - id: record_number
      type: u2
    - id: record_length
      type: u2
    - id: record_data
      size: record_length * 2
    
  mask_write_register_function:
    seq:
    - id: reference_address
      type: u2  
    - id: and_mask
      type: u2
    - id: or_mask
      type: u2
  read_write_multiuple_registers_function:
    seq:
    - id: read_starting_address
      type: u2
    - id: quantity_to_read
      type: u2
    - id: write_starting_address
      type: u2
    - id: qunatity_to_write
      type: u2
    - id: write_byte_count
      type: u1
    - id: write_register_value
      size: write_byte_count
      
      
  read_fifo_queue_function:
    seq:
    - id: fifo_pointer_address
      type: u2 
      
  read_device_identification_function:   
    seq:
    - id: mei_type
      type: u1 
    - id: read_device_id_code
      type: u1
    - id: object_id
      type: u1

enums:
  modbus_function_code:
    1: read_coil_status
    2: read_input_status
    3: read_holding_register
    4: read_input_registers
    5: write_single_coil
    6: write_single_register
    7: read_exception_status
    8: diagnostic
    11: get_com_event_counter
    12: get_com_event_log
    15: write_multiple_coils
    16: write_multiple_registers
    17: report_slave_id
    20: read_file_record
    21: write_file_record
    22: mask_write_register
    23: read_write_multiuple_registers
    24: read_fifo_queue
    43: read_device_identification