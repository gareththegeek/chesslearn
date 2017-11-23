import tensorflow as tf


class DataReader:

    def __init_iterator(self, filename):
        self.__iterator = tf.python_io.tf_record_iterator(filename)

    def __init__(self, filename):
        self.__filename = filename
        self.__init_iterator(filename)

    def get_example_count(self):
        count = 0
        while next(self.__iterator, None) is not None:
            count += 1

        self.__init_iterator(self.__filename)
        
        return count

    def get_example(self):
        serialised_example = next(self.__iterator, None)

        if serialised_example is None:
            self.__init_iterator(self.__filename)
            serialised_example = next(self.__iterator, None)

        # example = tf.train.Example()
        # example.ParseFromString(serialised_example)
        
        features = tf.parse_single_example(
            serialised_example,
            features={
                "label": tf.FixedLenFeature([1], tf.float32),
                "features": tf.FixedLenFeature([356], tf.float32)
            })
        
        label = features["label"]
        material = features["features"][:20]
        piece = features["features"][20:228]
        square = features["features"][228:]

        return label, material, piece, square

    def get_batch(self, size):
        batch = []
        for i in range(size):
            batch.append(self.get_example())
        return batch