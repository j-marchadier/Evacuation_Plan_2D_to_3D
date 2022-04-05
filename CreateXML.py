from xml.dom import minidom
import os
from pathlib import Path

def createXML(paths,sizeInput,objectInput):
    pathAbs = str(Path(__file__).parent.absolute())
    root = minidom.Document()

    # FOLDER
    annotation = root.createElement('annotation')

    # FOLDER
    folder = root.createElement('folder')
    folder.appendChild(root.createTextNode(paths.split("/")[-2]))
    annotation.appendChild(folder)

    # FILENAME
    filename = root.createElement('filename')
    filename.appendChild(root.createTextNode(paths.split("/")[-1].split(".")[0]))
    annotation.appendChild(filename)

    # PATH
    path = root.createElement('path')
    path.appendChild(root.createTextNode(pathAbs+"/"+paths))
    annotation.appendChild(path)

    # SOURCE
    source = root.createElement('source')

    # DATABASE
    database = root.createElement('database')
    database.appendChild(root.createTextNode('Unknown'))
    source.appendChild(database)

    annotation.appendChild(source)

    # SIZE
    size = root.createElement('size')

    listSize=("width","height","depth")
    for i in listSize:
        l = listSize.index(i)
        a = root.createElement(i)
        a.appendChild(root.createTextNode(str(sizeInput[l])))
        size.appendChild(a)
    annotation.appendChild(size)

    # SEGMENTED
    segmented = root.createElement('segmented')
    segmented.appendChild(root.createTextNode('0'))
    annotation.appendChild(segmented)

    for obj in objectInput :
        ##### Start OBJECT ######
        # OBJECT
        object = root.createElement('object')

        # NAME
        name = root.createElement('name')
        name.appendChild(root.createTextNode('legend'))
        object.appendChild(name)

        # POSE
        pose = root.createElement('pose')
        pose.appendChild(root.createTextNode('Unspectified'))
        object.appendChild(pose)

        # TRUNCATED
        truncated = root.createElement('pose')
        truncated.appendChild(root.createTextNode('0'))
        object.appendChild(truncated)

        # DIFFICULT
        difficult = root.createElement('difficult')
        difficult.appendChild(root.createTextNode('0'))
        object.appendChild(difficult)

        # BNDBOX
        bndbox = root.createElement('bndbox')

        coord = ("xmin","ymin","xmax","ymax")
        for i in range(4):
            a = root.createElement(coord[i])
            a.appendChild(root.createTextNode(str(obj[i])))
            bndbox.appendChild(a)

        object.appendChild(bndbox)
        annotation.appendChild(object)
    #########################

    root.appendChild(annotation)

    xml_str = root.toprettyxml(indent ="\t")


    save_path_file = str(pathAbs) +"/"+ paths.split("/")[0]+"/"+paths.split("/")[1]+"/"+paths.split("/")[-1].split(".")[0]+".xml"
    with open(save_path_file, "w") as f:
        f.write(xml_str)