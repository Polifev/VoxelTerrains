using NUnit.Framework;
using System.IO;
using UnityEngine;
using VoxelTerrains.Model;

[TestFixture]
internal class TestFileSystemChunkRepository
{
    private static readonly string TEST_DIR_ROOT = ".\\Assets\\Data\\Tests\\TestFileSystemChunkRepository";

    [Test]
    public void KnowIfAChunkIsSaved()
    {
        var dir = Path.Combine(TEST_DIR_ROOT, "savegame_01");
        var repository = new FileSystemChunkRepository(dir);
        Assert.IsTrue(repository.HasChunk(new Vector3Int(0, 1, 2)));
    }

    [Test]
    public void SaveAndLoad()
    {
        var dir = Path.Combine(TEST_DIR_ROOT, "savegame_01");
        var repository = new FileSystemChunkRepository(dir);
        var c = new Chunk(new float[] { 1.1f, 1.2f, 1.3f, 1.4f, 2.1f, 2.2f, 2.3f, 2.4f });
        var index = new Vector3Int(5, 6, 7);
        repository.SaveChunk(index, c);
        Assert.IsTrue(repository.HasChunk(index));
        Assert.That(repository.LoadChunk(index).Data, Is.EquivalentTo(c.Data));
        File.Delete(repository.GetFilePath(index));
    }
}
