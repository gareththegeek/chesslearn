# chesslearn

A neural network chess engine inspired by Giraffe ([White Paper](https://arxiv.org/pdf/1509.01549.pdf)) and implemented using [TensorFlow](https://www.tensorflow.org/)

This project utilises [KingBase pgn chess database](http://www.kingbase-chess.net/) to provide training data for the neural network and [Stockfish](https://stockfishchess.org/) to label the positions within the database to allow supervised learning.

## Process

To produce labelled training data for the neural network, a number of steps must be performed.

## 1. Convert PGN into FEN

The Chess.Featuriser application written in C# is capable of parsing PGN files and writing all unique positions which occur within the PGN file to a csv using FEN notation.

To do this, the following command should be executed against each PGN file:

`chess.featuriser -i "pgn file to parse" -o "csv file to create" -f -u`

## 2. Label FENs

The next step of the process is to label each of the unique positions contained within the csv files produced by step 1.  For the KingBase pgn chess database, 15 files were produced.

The labeliser.py script utilises [pystockfish](https://github.com/iamjarret/pystockfish) to execute Stockfish to evaluate each position and writes the position and Stockfish score to a second csv file.  For this evaluation, I ran Stockfish at a depth of 10 in order to reduce execution time.

To label the FENs execute the following command against the labeliser script:

`python labeliser.py "fen csv file to read from" "output csv to write to"`

## 3. Generate Features

Once the labelled positions are stored within csv files, the next step is to have the chess.featuriser generate features for each labelled position and output these to another csv file.  The features which are produced are based on the features used by the Giraffe engine (see the linked whitepaper for details).  The features produced are:

* Whose turn is it
* Castling rights for black and white (long and short)
* Number of each type of piece each player has
* Position of each piece along with the weakest attack and defender of the piece
* The distance that each sliding piece (Queen, Rook, Bishop) can move in each direction
* The weakest attacker and defender of each square on the board

To generate features from a labelled position csv use the following command:

`chess.featuriser -i "csv containing labelled positions" -o "csv to write to" -e -s`

## 4. Statistics

The following step will normalise the data (move all data to the range 0-1) to prepare the data for the neural network.  In order to do that, this step finds the minimum and maximum value in each column of the csv files generated so far.

This is achieved by executing the statifier.py script.  The results will be written to Statistics.csv and the list of files to process is defined within the script in an inputFiles array.  It is necessary to modify the script directly to specify the files to be processed.

`python statifier.py`

## 5. Normalisation

Normalisation of the data is performed by the normaliser.py script.  As with the statifier.py script, the input and output files are hard coded into the script.  In addition to normalising the data, this step combines the data from all 15 feature files and combines it into two files, the train and test files.  Each position has a 20% chance of being included in the test file, otherwise it will be included in the train file.

Loading this data to and from csv is no longer practical. It takes a very large (greater than 100Gb) amount of disk space and it is not possible to deserialise into memory when using tensorflow.  Instead, this step serialises the data to .tfrecords files. This binary format allows tensorflow to efficiently load data in batches and avoids the memory and performance issues associated with csv format.

See this article for details [TensorFlow Data Input (Part 1): Placeholders, Protobufs & Queues](https://indico.io/blog/tensorflow-data-inputs-part1-placeholders-protobufs-queues/)

Once this step is complete, the data is ready to be presented to the neural network for training.

`python normaliser.py`

## 6. Tensorflow Training

TODO!
