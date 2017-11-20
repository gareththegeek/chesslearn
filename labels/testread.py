# Test reading from tfrecords files

import tensorflow as tf

filename = "test.tfrecords"
iterator = tf.python_io.tf_record_iterator(filename)

serialised_example = next(iterator, None)

#for serialized_example in tf.python_io.tf_record_iterator(filename):
example = tf.train.Example()
example.ParseFromString(serialised_example)

# traverse the Example format to get data
features = example.features.feature['features'].float_list.value
labels = example.features.feature['label'].float_list.value
# do something
print(features)
print(labels)