import csv

inputFiles = [
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

    print("Processing file " + inputFile)

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
