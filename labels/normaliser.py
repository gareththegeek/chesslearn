import csv
import sys
import random
import numpy as np

inputFiles = [
    # "a.csv",
    # "b.csv",
    # "c.csv"
    "KingBase2017-A00-A39 a-LABELLED-FEATURES.csv",
    "KingBase2017-A40-A79 a-LABELLED-FEATURES.csv",
    "KingBase2017-A80-A99 a-LABELLED-FEATURES.csv",
    "KingBase2017-B00-B19 a-LABELLED-FEATURES.csv",
    "KingBase2017-B20-B49 a-LABELLED-FEATURES.csv",
    "KingBase2017-B50-B99 a-LABELLED-FEATURES.csv",
    "KingBase2017-C00-C19 a-LABELLED-FEATURES.csv",
    "KingBase2017-C20-C59 a-LABELLED-FEATURES.csv",
    "KingBase2017-C60-C99 a-LABELLED-FEATURES.csv",
    "KingBase2017-D00-D29 a-LABELLED-FEATURES.csv",
    "KingBase2017-D30-D69 a-LABLLED-FEATURES.csv",
    "KingBase2017-D70-D99 a-LABELLED-FEATURES.csv",
    "KingBase2017-E00-E19 a-LABELLED-FEATURES.csv",
    "KingBase2017-E20-E59 a-LABELLED-FEATURES.csv",
    "KingBase2017-E60-E99 a-LABELLED-FEATURES.csv"
]
statsFile = "Statistics.csv"
trainFile = "Train.csv"
testFile = "Test.csv"

# statsFile = "s.csv"
# trainFile = "tr.csv"
# testFile = "te.csv"

testProbability = 0.2
reportEvery = 1

def processRow(wtrain, wtest, rin):
    for row in rin:
        inputs = np.asarray(next(rin, None), dtype=float)
        outputs = (inputs - minimums) / ranges
        if random.random() < testProbability:
            wtest.writerow(outputs)
        else:
            wtrain.writerow(outputs)

def processFiles(wtrain, wtest):
    for inputFile in inputFiles:
        print("Processing file " + inputFile)
        with open(inputFile, "rt") as fin:
            rin = csv.reader(fin, delimiter=",")
            next(rin, None)
            processRow(wtrain, wtest, rin)

print("Reading stats")

with open(statsFile, "rt") as fstat:
    rstat = csv.reader(fstat, delimiter=",")
    headings = next(rstat, None)
    minimums = np.asarray(next(rstat, None), dtype=float)
    maximums = np.asarray(next(rstat, None), dtype=float)
    ranges = maximums - minimums
    if np.any(ranges == 0):
        print("Zero range detected - this will cause divide by zeroes")
        np.seterr(divide='ignore', invalid='ignore')

print("Read minimums and ranges")

with open(trainFile, "w", newline="") as ftrain:
    wtrain = csv.writer(ftrain)
    wtrain.writerow(headings)
    with open(testFile, "w", newline="") as ftest:
        wtest = csv.writer(ftest)
        wtest.writerow(headings)

        processFiles(wtrain, wtest)

print("Done")
