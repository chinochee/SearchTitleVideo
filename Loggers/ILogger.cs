﻿namespace SearchTitleVideo.Loggers;

public interface ILogger
{
    public void Write(string message);
    public void Close();
}