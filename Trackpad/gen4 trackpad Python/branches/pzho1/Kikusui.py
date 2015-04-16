import comtypes.client
import comtypes


PS1=comtypes.client.CreateObject('Kikusui4800.Kikusui4800')

if PS1 is None:
    print "ps1 is none"

print PS1.Initialize("USB0::0x0B3E::0x1014::NB003730::0::INSTR",True,True,"")
print PS1.Identity.InstrumentModel