import tensorflow as tf
from DataReader import DataReader

testFile = "Test.tfrecords"

r = DataReader(testFile)

#print("Example count: " + str(r.get_example_count()))

batch = r.get_batch(3)

sess = tf.Session()
init = tf.global_variables_initializer()
sess.run(init)
#tf.train.start_queue_runners(sess=sess)

for i in range(3):
    print("Example " + str(i))
    b = batch[i]
    print("Score (label)")
    print(sess.run(b[0]))
    print("Material Centric (features)")
    print(sess.run(b[1]))
    print("Piece Centric (features)")
    print(sess.run(b[2]))
    print("Square Centric (features)")
    print(sess.run(b[3]))
