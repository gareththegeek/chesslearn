import csv
import sys
import random

inputFiles = [
    "a.csv", "b.csv", "c.csv"
]
statsFile = "stats.csv"

print("Generating stats")

columnCount = 0
headings = []

print("Counting columns")

with open(inputFiles[0], "rt") as fin:
    rin = csv.reader(fin, delimiter=",")
    headings = next(rin, None)
    columnCount = len(headings)

print(str(columnCount) + " columns found")

mins = [1000] * columnCount
maxs = [-1000] * columnCount

for inputFile in inputFiles:
    with open(inputFile, "rt") as fin:
        rin = csv.reader(fin, delimiter=",")
        next(rin, None)

        for row in rin:
            if columnCount != len(row):
                print("Column number mismatch in file " + inputFile)
                continue

            i = 0
            for value in row:
                if float(value) < mins[i]:
                    mins[i] = float(value)
                if float(value) > maxs[i]:
                    maxs[i] = float(value)
                i += 1

print("Finshed generating stats")
print("Mins: " + ",".join(str(m) for m in mins))
print("Maxs: " + ",".join(str(m) for m in maxs))

print("Writing stat file: " + statsFile)

with open(statsFile, "w", newline="") as fstat:
    wstat = csv.writer(fstat)
    wstat.writerow(headings)
    wstat.writerow(mins)
    wstat.writerow(maxs)

print("Wrote stat file")
