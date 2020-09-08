meta:
  id: dns_packet
  title: DNS (Domain Name Service) packet
  license: CC0-1.0
  endian: be
doc: |
  Implements DNS packet decoder.
seq:
  - id: transaction_id
    doc: "ID to keep track of request/responces"
    type: u2
  - id: flags
    type: packet_flags
  - id: qdcount
    doc: "How many questions are there"
    type: u2
  - id: ancount
    doc: "Number of resource records answering the question"
    type: u2
  - id: nscount
    doc: "Number of resource records pointing toward an authority"
    type: u2
  - id: arcount
    doc: "Number of resource records holding additional information"
    type: u2
  - id: queries
    type: query
    repeat: expr
    repeat-expr: qdcount
  - id: answers
    type: answer
    repeat: expr
    repeat-expr: ancount
types:
  query:
    seq: 
      - id: name
        type: domain_name
      - id: type
        type: u2
        enum: record_type
      - id: query_class
        type: u2
        enum: class_type
  answer:
    seq:
      - id: name
        type: domain_name
      - id: type
        type: u2
        enum: record_type
      - id: answer_class
        type: u2
        enum: class_type
      - id: ttl
        doc: "Time to live (in seconds)"
        type: s4
      - id: rdlength
        doc: "Length in octets of the following payload"
        type: u2
      - id: rdata
        type:
          switch-on: type
          cases:
            'record_type::ptr': ptr_record
            'record_type::a' : a_record
            'record_type::aaaa' : aaaa_record
            'record_type::cname' : cname_record
            'record_type::mx' : mx_record
            'record_type::ns' : ns_record            
  ptr_record:
    seq:                
      - id: hostname
        type: domain_name
  a_record:        
    seq:
      - id: address
        size: 4
  aaaa_record:        
    seq:
      - id: address
        size: 16        
  cname_record:
    seq:        
      - id: hostname 
        type: domain_name
  ns_record:
    seq:        
      - id: hostname 
        type: domain_name        
  mx_record:
    seq:        
      - id: priority 
        type: s2
      - id: hostname
        type: domain_name        
  domain_name:
    seq:
      - id: labels
        type: label
        repeat: until
        doc: "Repeat until the length is 0 or it is a pointer (bit-hack to get around lack of OR operator)"
        repeat-until: "_.length == 0 or _.length == 0b1100_0000"
  label:
    seq:
      - id: length
        doc: "RFC1035 4.1.4: If the first two bits are raised it's a pointer-offset to a previously defined name"
        type: u1
      - id: pointer
        if: "is_pointer"
        type: pointer_struct
      - id: name
        if: "not is_pointer"
        doc: "Otherwise its a string the length of the length value"
        type: str
        encoding: "ASCII"
        size: length
    instances:
      is_pointer:
        value: length == 0b1100_0000
  pointer_struct:
    seq:
      - id: offset
        doc: "Read one byte, then offset to that position, read one domain-name and return"
        type: u1
    instances:
      contents:
        io: _root._io
        pos: offset
        type: domain_name
  packet_flags:
    seq:
      - id: flag
        type: u2
    instances:
      qr:
        value: (flag & 0b1000_0000_0000_0000) >> 15
      opcode:
        value: (flag & 0b0111_1000_0000_0000) >> 11
      aa:
        value: (flag & 0b0000_0100_0000_0000) >> 10
      tc:
        value: (flag & 0b0000_0010_0000_0000) >> 9
      rd:
        value: (flag & 0b0000_0001_0000_0000) >> 8
      ra:
        value: (flag & 0b0000_0000_1000_0000) >> 7
      z:
        value: (flag & 0b0000_0000_0100_0000) >> 6
      ad:
        value: (flag & 0b0000_0000_0010_0000) >> 5
      cd:
        value: (flag & 0b0000_0000_0001_0000) >> 4
      rcode:
        value: (flag & 0b0000_0000_0000_1111) >> 0
        
enums:
  class_type:
    1: in_class
    2: cs
    3: ch
    4: hs
  record_type:
    1: a
    2: ns
    3: md
    4: mf
    5: cname
    6: soe
    7: mb
    8: mg
    9: mr
    10: "null"
    11: wks
    12: ptr
    13: hinfo
    14: minfo
    15: mx
    16: txt
    28: aaaa