using AdventOfCode2021.Util;
using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(16, "Packet Decoder")]
    class Day16 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual((long) 16, Part1("8A004A801A8002F478"));
            Assert.AreEqual((long) 12, Part1("620080001611562C8802118E34"));
            Assert.AreEqual((long) 23, Part1("C0015000016115A2E0802F182340"));
            Assert.AreEqual((long) 31, Part1("A0016C880162017C3686B18A3D4780"));
        }

        public override void Part2Test()
        {
            Assert.AreEqual((long) 3, Part2("C200B40A82"));
            Assert.AreEqual((long) 54, Part2("04005AC33890"));
            Assert.AreEqual((long) 7, Part2("880086C3E88112"));
            Assert.AreEqual((long) 9, Part2("CE00C43D881120"));
            Assert.AreEqual((long) 1, Part2("D8005AC2A8F0"));
            Assert.AreEqual((long) 0, Part2("F600BC2D8F"));
            Assert.AreEqual((long) 0, Part2("9C005AC2F8F0"));
            Assert.AreEqual((long) 1, Part2("9C0141080250320F1802104A08"));
        }

        public override object Part1(string input)
        {
            var reader = new BReader(input);
            return new VersionSumVisitor()
                .Visit(Packet.Read(reader));
        }

        public override object Part2(string input)
        {
            var reader = new BReader(input);
            return new EvaluationVisitor()
                .Visit(Packet.Read(reader));
        }

        private class VersionSumVisitor : IVisitor<long>
        {
            public long Visit(IVisitable visitable) => visitable.Accept(this);

            public long Visit(LiteralNumberPacket packet) => packet.Header.Version;

            public long Visit(OperatorPacket packet)
            {
                return packet.Header.Version + packet.SubPackets
                    .Sum(Visit);
            }
        }

        private class EvaluationVisitor : IVisitor<long>
        {
            public long Visit(IVisitable visitable) => visitable.Accept(this);

            public long Visit(LiteralNumberPacket packet) => packet.Value;

            public long Visit(OperatorPacket packet)
            {
                checked
                {
                    var packets = packet.SubPackets;
                    switch (packet.Header.TypeId)
                    {
                        case 0: return packets.Sum(Visit);
                        case 1: return packets.Aggregate(1L, (x, y) => x * Visit(y));
                        case 2: return packets.Min(Visit);
                        case 3: return packets.Max(Visit);
                        case 5: return Visit(packets[0]) > Visit(packets[1]) ? 1 : 0;
                        case 6: return Visit(packets[0]) < Visit(packets[1]) ? 1 : 0;
                        case 7: return Visit(packets[0]) == Visit(packets[1]) ? 1 : 0;
                        default: throw new UnreachableCodeException();
                    }
                }
            }
        }

        private interface IVisitable
        {
            T Accept<T>(IVisitor<T> visitor);
        }

        private interface IVisitor<out T>
        {
            T Visit(IVisitable visitable);
            T Visit(OperatorPacket packet);
            T Visit(LiteralNumberPacket packet);
        }

        private abstract class Packet : IVisitable
        {
            public Header Header { get; init; }
            public Length Length { get; init; }

            public static Packet Read(BReader reader)
            {
                var header = Header.Read(reader);

                return header.TypeId switch
                {
                    4 => LiteralNumberPacket.Read(header, reader),
                    _ => OperatorPacket.Read(header, reader)
                };
            }

            public abstract T Accept<T>(IVisitor<T> visitor);
        }

        private class LiteralNumberPacket : Packet
        {
            public long Value { get; init; }

            public static LiteralNumberPacket Read(Header header, BReader reader)
            {
                return new LiteralNumberPacket
                {
                    Header = header,
                    Value = reader.ReadLiteral(),
                };
            }

            public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
        }

        private class OperatorPacket : Packet
        {
            public IReadOnlyList<Packet> SubPackets { get; init; }

            public static OperatorPacket Read(Header header, BReader reader)
            {
                var length = Length.Read(reader);
                return new OperatorPacket
                {
                    Header = header,
                    Length = length,
                    SubPackets = ReadSubPackets(length, reader)
                };
            }

            private static List<Packet> ReadSubPackets(Length length, BReader reader)
            {
                return length.TypeId switch
                {
                    0 => ReadPacketsByBitLength(length.Value, reader),
                    1 => ReadPacketsByCount(length.Value, reader),
                    _ => throw new UnreachableCodeException()
                };
            }

            private static List<Packet> ReadPacketsByCount(int count, BReader reader)
            {
                var result = new List<Packet>();
                for (int i = 0; i < count; i++)
                    result.Add(Packet.Read(reader));
                return result;
            }

            private static List<Packet> ReadPacketsByBitLength(int length, BReader reader)
            {
                var result = new List<Packet>();
                while (length > 0)
                {
                    int pos = reader.Position;
                    result.Add(Read(reader));
                    length -= reader.Position - pos;
                }
                return result;
            }

            public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
        }

        private struct Header
        {
            public int Version { get; init; }
            public int TypeId { get; init; }

            public static Header Read(BReader reader)
            {
                return new Header
                {
                    Version = (int) reader.ReadInt(bits: 3),
                    TypeId = (int) reader.ReadInt(bits: 3),
                };
            }
        }

        private struct Length
        {
            public int TypeId { get; init; }
            public int Value { get; init; }

            public static Length Read(BReader reader)
            {
                int type = (int) reader.ReadInt(bits: 1);
                int len = type == 0 ? 15 : 11;

                return new Length
                {
                    TypeId = type,
                    Value = (int) reader.ReadInt(bits: len),
                };
            }
        }

        private class BReader
        {
            public int Position { get; private set; }

            public BReader(string hex)
            {
                data = hex.Trim()
                    .SelectMany(x => decode[x])
                   .ToArray();
            }

            public long ReadInt(int bits, long startWith = 0)
            {
                for (int i = 0; i < bits; i++)
                    startWith = (startWith << 1) | (long) data[Position++];
                return startWith;
            }

            public long ReadLiteral()
            {
                long result = 0;
                bool quit;

                do
                {
                    quit = ReadInt(bits: 1) == 0;
                    result = ReadInt(4, startWith: result);
                } while (!quit);

                return result;
            }

            private readonly int[] data;

            private static readonly Dictionary<char, int[]> decode = new()
            {
                { '0', new[] { 0, 0, 0, 0 } },
                { '1', new[] { 0, 0, 0, 1 } },
                { '2', new[] { 0, 0, 1, 0 } },
                { '3', new[] { 0, 0, 1, 1 } },
                { '4', new[] { 0, 1, 0, 0 } },
                { '5', new[] { 0, 1, 0, 1 } },
                { '6', new[] { 0, 1, 1, 0 } },
                { '7', new[] { 0, 1, 1, 1 } },
                { '8', new[] { 1, 0, 0, 0 } },
                { '9', new[] { 1, 0, 0, 1 } },
                { 'A', new[] { 1, 0, 1, 0 } },
                { 'B', new[] { 1, 0, 1, 1 } },
                { 'C', new[] { 1, 1, 0, 0 } },
                { 'D', new[] { 1, 1, 0, 1 } },
                { 'E', new[] { 1, 1, 1, 0 } },
                { 'F', new[] { 1, 1, 1, 1 } },
            };
        }
    }
}
