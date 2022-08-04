import os
import pathlib
import shutil
import xml.etree.ElementTree as xml
import time

# https://stackoverflow.com/questions/3095434/inserting-newlines-in-xml-file-generated-via-xml-etree-elementtree-in-python
def XMLIndent(elem, level=0):
	i = "\n" + level*"	"
	if len(elem):
		if not elem.text or not elem.text.strip():
			elem.text = i + "  "
		if not elem.tail or not elem.tail.strip():
			elem.tail = i
		for elem in elem:
			XMLIndent(elem, level+1)
		if not elem.tail or not elem.tail.strip():
			elem.tail = i
	else:
		if level and (not elem.tail or not elem.tail.strip()):
			elem.tail = i

def GetMonoXMLHeader(_type):
	root = xml.Element('XnaContent')
	root.attrib['xmlns:ns' ] = 'Microsoft.Xna.Framework'
	asset = xml.SubElement(root, 'Asset')
	asset.attrib['Type'] = _type
	return root, asset

def GetXMLSchemaHeader(_type, namespace):
	root = xml.Element('XnaContent')
	root.attrib['xmlns:XMLSchema'] = namespace + '.XMLSchema'
	asset = xml.SubElement(root, 'Asset')
	asset.attrib['Type'] = 'XMLSchema:' + _type
	return root, asset

def ExportXml(path, root):
	XMLIndent(root)
	f = open(path, 'wb')
	tree = xml.ElementTree(root)
	tree.write(f, 'UTF-8', xml_declaration=True, short_empty_elements=False )
	f.close()


def SerialiseNames(root, names):
	item = xml.SubElement(root, 'names')
	for x in names:
		xml.SubElement(item, 'Item').text = x
	return item

def SerialisePaths(root, paths):
	item = xml.SubElement(root, 'paths')
	for x in paths:
		xml.SubElement(item, 'Item').text = x
	return item

def RecurseDir(_root, _dir):
	
	for n in os.listdir(_dir):
		
		_sd = os.path.join(_dir, n)
		_rsd = os.path.relpath(_sd, _root)
					
		if os.path.isfile(_sd) and _root != _dir:
			if os.path.splitext(n)[0] in names:
				raise Exception("Asset Name Conflict: {0}".format(n))
			names.append(os.path.splitext(n)[0])
			paths.append(os.path.splitext(_rsd)[0])
			
		elif os.path.isdir(_sd) and n != "obj" and n != "bin":
			RecurseDir(_root, _sd)


this_dir = os.path.dirname(os.path.abspath(__file__))
#content_dir = os.path.join(this_dir, 'Assets')

# level stuff
names = []
paths = []

RecurseDir(this_dir, this_dir)

root, asset = GetMonoXMLHeader('Tide.XMLSchema.FContentMappings')
SerialiseNames(asset, names)
SerialisePaths(asset, paths)

outfile = os.path.join(this_dir, 'mappings.xml')
ExportXml(outfile, root)

print("mappings.xml Generated")