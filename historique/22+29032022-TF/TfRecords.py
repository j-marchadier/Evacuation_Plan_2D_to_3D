import os  # used for directory operations
import tensorflow as tf
from PIL import Image  # used to read images from directory

dict_logos = {"BT": 0, "exit": 1, "extincteur": 2, "incendie": 3, "porte": 4}
# my file path, just like the picture above
cwd = "./data/"
# the tfrecord file path, you need to create the folder yourself
recordPath = "tfrecord/"
# the best number of images stored in each tfrecord file
bestNum = 1000
# the index of images flowing into each tfrecord file
num = 0
# the index of the tfrecord file
recordFileNum = 0
# the number of classes of images
keys = [str(i) for i in dict_logos.keys()]
values = [i for i in list(range(1,6))]
classes = dict(zip(keys, values))
# name format of the tfrecord files
recordFileName = ("train.tfrecords-%.3d" % recordFileNum)
# tfrecord file writer
writer = tf.io.TFRecordWriter(recordPath + recordFileName)

print("Creating the 000 tfrecord file")
for name, label in classes.items():
    print(name)
    print(label)
    class_path = os.path.join(cwd, name)
    for img_name in os.listdir(class_path):
        num += 1
        if num > bestNum:
            num = 1
            recordFileNum += 1
            writer = tf.io.TFRecordWriter(recordPath + recordFileNum)
            print("Creating the %.3d tfrecord file" % recordFileNum)
        img_path = os.path.join(class_path, img_name)
        img = Image.open(img_path, "r")
        img_raw = img.tobytes()
        example = tf.train.Example(features=tf.train.Features(feature={
            "img_raw": tf.train.Feature(bytes_list=tf.train.BytesList(value=[img_raw])),
            "label": tf.train.Feature(int64_list=tf.train.Int64List(value=[label]))}))
        writer.write(example.SerializeToString())
writer.close()
