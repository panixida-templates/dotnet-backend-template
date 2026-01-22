using Bl.Implementations.Core;
using Bl.Interfaces;

using Dal.S3.Interfaces;

namespace Bl.Implementations;

public sealed class AvatarsBl(IAvatarsStorage avatarsStorage) :
    BaseStorageBl(avatarsStorage),
    IAvatarsBl
{
}
