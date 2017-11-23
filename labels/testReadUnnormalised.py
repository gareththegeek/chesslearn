import csv
import numpy as np

statFile = "data\\Statistics.csv"
inputFile = "data\\KingBase2017-A00-A39 a-LABELLED-FEATURES.csv"
outputFile = "data\\debug.csv"

with open(statFile, "rt") as fstat:
    rstat = csv.reader(fstat, delimiter=",")
    headings = next(rstat, None)
    minimums = np.asarray(next(rstat, None), dtype=float)
    maximums = np.asarray(next(rstat, None), dtype=float)
    ranges = maximums - minimums
    for i in np.where(ranges == 0):
        ranges[i] = 1

with open(outputFile, "w", newline="") as fout:
    with open(inputFile, "rt") as fin:
        rin = csv.reader(fin, delimiter=",")
        wout = csv.writer(fout)
        wout.writerow(next(rin, None))
        
        for row in rin:
            wout.writerow(row)

            inputs = np.asarray(row, dtype=float)
            outputs = (inputs - minimums) / ranges

            wout.writerow(outputs)
            input("Press enter")