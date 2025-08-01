﻿using System;
using System.Collections.Generic;

namespace GitHubDataFetcher.Mappers;

public static class LanguageIconMapper
{
    private static readonly Dictionary<string, string> Mappings =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Python",           "logos-python"        },
            { "Jupyter Notebook", "logos-jupyter"       },
            { "HTML",             "logos-html-5"        },
            { "CSS",              "logos-css-3"         },
            { "JavaScript",       "logos-javascript"    },
            { "C#",               "devicon:csharp"      },
            { "Java",             "logos-java"          },
            { "Shell",            "simple-icons:shell"  },
            { "Ruby",             "logos-ruby"          },
            { "PHP",              "logos-php"           },
            { "Dockerfile",       "simple-icons:docker" },
            { "Rust",             "logos-rust"          }
        };

    public static string GetIconifyIdentifier(string languageName)
        => Mappings.TryGetValue(languageName, out var iconifyIdentifier)
            ? iconifyIdentifier
            : string.Empty;
}
