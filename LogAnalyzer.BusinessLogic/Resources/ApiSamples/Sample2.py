LogAnalyzer.WritelnLog("Total count of entries: " + str(LogAnalyzer.Entries.Count))

for i in range(0, 10):
    entry = LogAnalyzer.Entries[i]

    LogAnalyzer.WritelnLog("Index: " + str(entry.Index));
    LogAnalyzer.WritelnLog("Date: " + str(entry.Date));
    LogAnalyzer.WritelnLog("Severity: " + str(entry.Severity));
    LogAnalyzer.WritelnLog("Message: " + str(entry.Message));
    LogAnalyzer.WritelnLog("Custom field: " + str(entry.Custom("Source")));
