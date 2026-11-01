# GraphDefined OID Registry

The object identifiers under the IANA Private Enterprise Number **60483**,
assigned to GraphDefined GmbH:

```
1.3.6.1.4.1.60483
iso(1) org(3) dod(6) internet(1) private(4) enterprise(1) graphdefined(60483)
```

This file is the registry. An object identifier under this arc means what this
file says it means, and nothing else.


## Rules

- **Recorded before used.** An arc is entered here before it appears in a
  certificate, a specification or code.
- **Never reassigned.** An arc that has been assigned keeps its meaning, even
  after it falls out of use: it is marked *withdrawn* and stays in the table.
  A new meaning gets a new arc.
- **Defined elsewhere, named here.** Each entry names the document that defines
  what the identifier carries: the syntax of an extension, the semantics of a
  policy. This file says *which* identifier, that document says *what*.


## Assignments

| OID | Name | Assigned | Status | Defined in |
|---|---|---|---|---|
| `1.3.6.1.4.1.60483.42` | EVQI | 2026-11-01 | assigned | [EVQI Transport §6.2][evqi-6.2] |
| `1.3.6.1.4.1.60483.42.1` | EVQI role extension (X.509v3 extension, one DER `UTF8String`) | 2026-11-01 | assigned | [EVQI Transport §6.2][evqi-6.2] |
| `1.3.6.1.4.1.60483.42.2` | certificate policy "EVQI Privileged Operations" | 2026-11-01 | assigned | [EVQI Transport §6.2][evqi-6.2] |
| `1.3.6.1.4.1.60483.42.3` | certificate policy "EVQI Key Agreement" | 2026-11-01 | assigned | [EVQI Transport §6.8][evqi-6.8] |

EVQI Transport is a draft. Its arc is fixed; what the identifiers under it
carry may still be refined there, and is not reassigned for it.


## Not yet listed

Arcs under 60483 that were assigned before this file existed are not listed
yet. Until they are, ask the maintainer before taking a new arc, rather than
taking the next free-looking number.


## References

- [IANA Private Enterprise Numbers](https://www.iana.org/assignments/enterprise-numbers/): 60483 – GraphDefined GmbH
- [EVQI Transport: Multi-Channel Communication between Charging Stations and CSMS][evqi]

[evqi]:     https://github.com/OpenChargingTechnology/Whitepapers/blob/master/EVQITransport/README.md
[evqi-6.2]: https://github.com/OpenChargingTechnology/Whitepapers/blob/master/EVQITransport/README.md#62-transfer-to-evqi
[evqi-6.8]: https://github.com/OpenChargingTechnology/Whitepapers/blob/master/EVQITransport/README.md#68-key-agreement-certificates
