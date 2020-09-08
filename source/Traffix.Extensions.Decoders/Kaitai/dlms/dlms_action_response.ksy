meta:
  id: dlms_action_response
  endian: be
  imports:
    - dlms_data
    - dlms_struct
seq:
    - id: response_type 
      type: u1
      enum: action_response_type
    - id: response
      type:
        switch-on: response_type
        cases: 
            'action_response_type::action_response_normal': action_response_normal
            'action_response_type::action_response_with_pblock': action_response_with_pblock
            'action_response_type::action_response_with_list': action_response_with_list
            'action_response_type::action_response_next_pblock': action_response_next_pblock
types:
    action_response_normal:
      seq:
        - id: invoke_id_and_priority
          type: dlms_struct::invoke_id_and_priority
        - id: single_response
          type: action_response_with_optional_data
    action_response_with_pblock:
        {}
    action_response_with_list:
        {}
    action_response_next_pblock:
        {}

    action_response_with_optional_data:
      seq: 
        - id: result 
          type: u1
          enum: action_result   
        - id: return_parameters 
          type: dlms_struct::get_data_result_optional
enums:
  action_response_type:
    1: action_response_normal
    2: action_response_with_pblock
    3: action_response_with_list
    4: action_response_next_pblock
  action_result:
    0: success
    1: hardware_fault
    2: temporary_failure
    3: read_write_denied
    4: object_undefined
    5: object_class_inconsistent
    11: object_unavailable
    12: type_unmatched
    13: scope_of_access_violated
    14: data_block_unavailable
    15: long_action_aborted
    16: no_long_action_in_progress
    250: other_reason
