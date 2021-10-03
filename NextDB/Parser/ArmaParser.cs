using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;

namespace NextDB.Parser
{
    public class ArmaParser
    {
        // ["Hello there Gernal'Kenobi",[[100,200,300],[200,300,400],[400,500,600]], true, "true", false, 123, [[[]]]]
        public ArmaValue ReadArmaValues(string input)
        {
            var data = input.AsSpan();
            if (data.IsEmpty) return null;

            if (data.StartsWith("["))
            {
                var armaArray = ReadArray(data[1..^1]);
                
                return armaArray;
            }

            if (data.StartsWith("\""))
            {
                return ReadStringValue(data);
            }

            if (TryReadBool(data, out var boolArmaValue))
            {
                return boolArmaValue!;
            }

            if (TryReadScalar(data, out var scalarArmaValue))
            {
                return scalarArmaValue!;
            }

            throw new ArgumentException("Invalid data");
        }

        public ArmaValue ReadStringValue(ReadOnlySpan<char> input)
        {
            var stringBuilder = new StringBuilder(input.ToString());
            stringBuilder.Replace("\"\"", "\"");

            return new ArmaValue(stringBuilder.ToString(), "STRING");
        }

        public ArmaValue ReadArray(ReadOnlySpan<char> input)
        {
            var armaValue = ArmaValue.GenerateArray();
            
            while (!input.IsEmpty)
            {
                if (input.StartsWith("["))
                {
                    var scopeCounter = 0;
                    for (var i = 0; i < input.Length; i++)
                    {
                        if (input[i] == ']')
                        {
                            scopeCounter--;
                            if (scopeCounter == 0)
                            {
                                armaValue.ArrayData.Add(ReadArray(input[1..i]));
                                input = input[(i+1)..];
                                break;
                            }
                        }

                        if (input[i] == '[')
                            scopeCounter++;
                    }

                    //throw new Exception($"Could not find end sequence of arma array: {input.ToString()}");
                }
                
                if (input.StartsWith("\""))
                {
                    //we need to find the ending sequence of a string e.g. """hello"" there"
                    //this means we need to find a quote that is not followed by another one

                    var matchFound = false;
                    
                    for (int i = 0; i < input.Length; i++)
                    {
                        int quoteCount = 0;
                        if (input[i] == '\"')
                        {
                            for (int j = i; j < input.Length; j++)
                            {
                                if (input[i] == '\"')
                                {
                                    quoteCount++;
                                }
                                else
                                {
                                    i = j;
                                    break;
                                }
                            }

                            if (i > 0 && quoteCount % 2 == 0)
                            {
                                armaValue.ArrayData.Add(ReadStringValue(input[..i]));
                                input = input[..i];
                                matchFound = true;
                                break;
                            }
                        }
                    }

                    if (!matchFound && input.Length > 1 && input[0] == '\"' && input[1] == '\"')
                    {
                        armaValue.ArrayData.Add(new ArmaValue("", "STRING"));
                        input = input[2..];
                        matchFound = true;
                    }

                    if (matchFound)
                        continue;
                    
                    throw new Exception("cannot find string end sequence");
                }

                if (input.StartsWith(","))
                {
                    input = input[1..];
                    continue;
                }

                var endSequenceIndex = input.IndexOfAny(']', ',');
                if (endSequenceIndex == -1) endSequenceIndex = input.Length;
                
                if (TryReadBool(input[..endSequenceIndex], out var boolArmaValue))
                {
                    armaValue.ArrayData.Add(boolArmaValue!);
                    input = input[endSequenceIndex..];
                    continue;
                }
                if (TryReadScalar(input[..endSequenceIndex], out var scalarArmaValue))
                {
                    armaValue.ArrayData.Add(scalarArmaValue!);
                    input = input[endSequenceIndex..];
                }
            }

            return armaValue;
        }

        public bool TryReadBool(ReadOnlySpan<char> input, out ArmaValue? armaValue)
        {
            if (bool.TryParse(input, out var result))
            {
                armaValue = new ArmaValue(result, "BOOL");
                return true;
            }

            armaValue = null;
            return false;
        }

        public bool TryReadScalar(ReadOnlySpan<char> input, out ArmaValue? armaValue)
        {
            if (float.TryParse(input, out var result))
            {
                armaValue = new ArmaValue(result, "SCALAR");
                return true;
            }

            armaValue = null;
            return false;
        }
    }
}