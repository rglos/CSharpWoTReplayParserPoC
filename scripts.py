import sys
sys.path.append("..\\..\\Lib")
import cPickle as pickle
import struct
from encodings import hex_codec
import json

def decode_details(data):
    detail = [
        "spotted",
        "killed",
        "hits",
        "he_hits",
        "pierced",
        "damageDealt",
        "damageAssisted",
        "crits",
        "fire"
    ]
    details = {}

    binlen = len(data) // 22
    for x in range(0, binlen):
        offset = 4*binlen + x*18
        vehic = struct.unpack('i', data[x*4:x*4+4])[0]
        detail_values = struct.unpack('hhhhhhhhh', data[offset:offset + 18])
        details[vehic] = dict(zip(detail, detail_values))
    return details


def load_details_blob(blob):
    try:
        results = pickle.loads(blob)
        if results:
            for k, v in results['vehicles'].items():
                results['vehicles'][k]['details'] = decode_details(v['details'])
            return json.dumps(results)
    except Exception:
        return "{}"