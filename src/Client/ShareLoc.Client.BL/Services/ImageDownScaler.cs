using SkiaSharp;

namespace ShareLoc.Client.BL.Services;

public sealed class ImageDownScaler
{
	private readonly int _maxSize;
	private readonly double[] _compressionLevels;

	public ImageDownScaler(int maxSize, int runCount, double initialCompressionLevel)
	{
		_maxSize = maxSize;

		_compressionLevels = new double[runCount];
		_compressionLevels[0] = initialCompressionLevel;
		for (int i = 1; i < runCount; i++)
		{
			_compressionLevels[i] = _compressionLevels[i - 1] / 2;
		}
	}

	public ImageDownScaler(int maxSize, double[] compressionLevels)
	{
		_maxSize = maxSize;
		_compressionLevels = compressionLevels;
	}

	public Task<byte[]> ScaleDownAsync(Stream imageStream, CancellationToken ct)
	{
		return Task.Run(() =>
		{
			var bufferCount = imageStream.Length;
			if (bufferCount < _maxSize)
			{
				using var newImageStream = new MemoryStream(_maxSize);
				imageStream.CopyToAsync(newImageStream, ct);
				return newImageStream.ToArray();
			}

			using var image = SKImage.FromEncodedData(imageStream);
			using var original = SKBitmap.FromImage(image);

			byte[] imageBuffer = [];
			foreach (var compressionLevel in _compressionLevels)
			{
				//size could be > max size
				imageBuffer = ScaleDown(original, bufferCount, compressionLevel);
				if (imageBuffer.Length > 0)
					return imageBuffer;
			}

			//100% size < max size
			return ScaleDown(original, bufferCount, null);
		}, ct);
	}

	private byte[] ScaleDown(SKBitmap original, long bufferCount, double? compressionLevel)
	{
		var bytesPerPixel = original.BytesPerPixel;

		var rawByteCount = original.Width * original.Height * bytesPerPixel;
		var compressionRatio = compressionLevel switch
		{
			null => 1,
			_ => (double)rawByteCount / bufferCount * compressionLevel
		};

		var newWidth2 = (double)(_maxSize * original.Width * compressionRatio) / (original.Height * bytesPerPixel);
		var newWidth = (int)Math.Sqrt(newWidth2);
		if (newWidth > original.Width)
			return [];

		var newHeight = newWidth * original.Height / original.Width;

		using var newImageStream = new MemoryStream(_maxSize);
		using var newBitmap = original.Resize(new SKImageInfo(newWidth, newHeight, original.ColorType, original.AlphaType, original.ColorSpace), SKFilterQuality.High);
		using var image = SKImage.FromBitmap(newBitmap);
		using var data = image.Encode(SKEncodedImageFormat.Png, 100);
		data.SaveTo(newImageStream);

		if (newImageStream.Length > _maxSize)
			return [];

		return newImageStream.ToArray();
	}
}
