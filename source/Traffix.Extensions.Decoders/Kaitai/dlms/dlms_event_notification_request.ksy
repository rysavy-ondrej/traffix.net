meta:
  id: dlms_event_notification_request
  endian: be
  imports:
    - dlms_data
    - dlms_struct
seq:
    - id: time  
      type: dlms_struct::cosem_date_time_optional
    - id: cosem_attribute_descriptor 
      type: dlms_struct::cosem_attribute_descriptor
    - id: attribute_value
      type: dlms_data 