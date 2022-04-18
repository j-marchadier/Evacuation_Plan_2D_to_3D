from urllib.parse import urlencode
from urllib.request import Request, urlopen

import requests

#url = 'http://localhost:10001/alert' # Set destination URL here
post_fields = {'timestamp': 00000,'reason': 'TRACKING',"coordinates":[1,2]}  # Set POST fields here

#request = Request(url, urlencode(post_fields).encode())
#json = urlopen(request).read().decode()
#print(json)

import json
r = requests.post('http://localhost:10001/alert', data = json.dumps(post_fields))

print(r)