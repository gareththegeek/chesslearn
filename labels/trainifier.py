import tflearn
from tflearn.data_utils import load_csv

trainFile = "Train.csv"
testFile = "Test.csv"

# print("Loading training data")
# train = load_csv(trainFile, has_header=False)
print("Loading test data")
test = load_csv(testFile, has_header=False)
print("Loading complete")
