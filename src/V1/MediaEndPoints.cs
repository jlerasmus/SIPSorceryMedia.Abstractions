﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SIPSorceryMedia.Abstractions.V1
{
    public delegate void AudioEncodedSampleDelegate(AudioCodecsEnum audioFormat, uint durationRtpUnits, byte[] sample);
    public delegate void VideoEncodedSampleDelegate(VideoCodecsEnum videoFormat, uint durationRtpUnits, byte[] sample);
    public delegate void RawAudioSampleDelegate(AudioSamplingRatesEnum samplingRate, uint durationMilliseconds, short[] sample);
    public delegate void RawVideoSampleDelegate(uint durationMilliseconds, int width, int height, byte[] rgb24Sample);
    public delegate void VideoSinkSampleDecodedDelegate(byte[] bmp, uint width, uint height, int stride);

    public enum AudioSamplingRatesEnum
    {
        Rate8KHz = 0,
        Rate16KHz = 1,
    }

    public enum AudioCodecsEnum
    {
        PCMU = 0,
        G722 = 9,
        PCMA = 8,
        OPUS = 111,

        Unknown = 999,
    }

    public enum VideoCodecsEnum
    {
        VP8 = 100,
        H264 = 102,

        Unknown = 999,
    }

    //public struct AudioFormat
    //{
    //    public string Name { get; set; }
    //    public int PayloadID { get; set; }
    //    public AudioCodecsEnum Codec { get; set; }
    //    public AudioSamplingRatesEnum Rate { get; set; }
    //    public int BitsPerSample { get; set; }
    //    public Dictionary<string, string> CustomProperties { get; set; }
    //}

    //public struct VideoFormat
    //{
    //    public string Name { get; set; }
    //    public int PayloadID { get; set; }
    //    public VideoCodecsEnum Codec { get; set; }
    //    public int FrameRate { get; set; }
    //    public int BitsPerSample { get; set; }
    //    public Dictionary<string, string> CustomProperties { get; set; }
    //}

    public class MediaEndPoints
    {
        public IAudioSource AudioSource { get; set; }
        public IAudioSink AudioSink { get; set; }
        public IVideoSource VideoSource { get; set; }
        public IVideoSink VideoSink { get; set; }
    }

    public interface IAudioEncoder
    {
        bool IsSupported(AudioCodecsEnum codec);

        byte[] EncodeAudio(byte[] pcm, AudioCodecsEnum codec, AudioSamplingRatesEnum sampleRate);
        
        byte[] EncodeAudio(short[] pcm, AudioCodecsEnum codec, AudioSamplingRatesEnum sampleRate);
     
        byte[] DecodeAudio(byte[] encodedSample, AudioCodecsEnum codec, AudioSamplingRatesEnum sampleRate);
    }

    public interface IAudioSource
    {
        event AudioEncodedSampleDelegate OnAudioSourceEncodedSample;

        event RawAudioSampleDelegate OnAudioSourceRawSample;

        Task PauseAudio();

        Task ResumeAudio();

        Task StartAudio();

        Task CloseAudio();

        List<AudioCodecsEnum> GetAudioSourceFormats();

        void SetAudioSourceFormat(AudioCodecsEnum audioFormat);
    }

    public interface IAudioSink
    {
        bool EncodedSamplesOnly { get; set; }

        AudioSamplingRatesEnum AudioPlaybackRate { get; set; }

        List<AudioCodecsEnum> GetAudioSinkFormats();

        void SetAudioSinkFormat(AudioCodecsEnum audioFormat);

        void GotAudioRtp(IPEndPoint remoteEndPoint, uint ssrc, uint seqnum, uint timestamp, int payloadID, bool marker, byte[] payload);
    }

    public interface IVideoSource
    {
        event VideoEncodedSampleDelegate OnVideoSourceEncodedSample;

        event RawVideoSampleDelegate OnVideoSourceRawSample;

        Task PauseVideo();

        Task ResumeVideo();

        Task StartVideo();

        Task CloseVideo();

        List<VideoCodecsEnum> GetVideoSourceFormats();

        void SetVideoSourceFormat(VideoCodecsEnum videoFormat);
    }

    public interface IVideoSink
    {
        /// <summary>
        /// This event will be fired by the sink after is decodes a video frame from the RTP stream.
        /// </summary>
        event VideoSinkSampleDecodedDelegate OnVideoSinkDecodedSample;

        void GotVideoRtp(IPEndPoint remoteEndPoint, uint ssrc, uint seqnum, uint timestamp, int payloadID, bool marker, byte[] payload);

        List<VideoCodecsEnum> GetVideoSinkFormats();

        void SetVideoSinkFormat(VideoCodecsEnum videoFormat);
    }
}