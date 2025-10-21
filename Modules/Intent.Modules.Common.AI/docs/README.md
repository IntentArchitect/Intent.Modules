# Intent.Common.AI

The `Intent.Common.AI` module provides shared infrastructure, settings, and metadata used across all AI-related modules in Intent Architect.

This module includes:

- Common configuration models for AI providers
- Standardized settings for prompts, token limits, and model selection
- Utilities and abstractions to streamline AI module development

> ℹ️ This module is intended to be used as a dependency for other AI modules and is not directly used for AI task execution.

## User Settings

AI provider details are configured in **User Settings**. These settings are user-specific, stored outside of the solution folder, and are not committed to source control.

The **User Settings** are available from the `User` menu in the top-right hand corner of Intent Architect:

![User Menu](images/user-settings-menu.png)

Within the **User Settings**, is an **AI Settings** section:

![AI Settings](images/user-settings.png)

### Available Settings

- **Provider**: The AI provider to use. Supported options:
  - [OpenAI](#openai-details)
  - [Azure OpenAI](#azure-openai-details)
  - [Anthropic](#anthropic-details)
  - [Open Router](#open-router-details)
  - [Google Gemini](#google-gemini-details)
  - [Ollama](#ollama-details)


- **Model**: The LLM model to use, specific to the selected provider.
- **API Key**: The API key used to authenticate with the provider.
  - You can store a separate key for each provider if needed.
  - Instead of storing them in here, you can fall back to using environment variables:
    - **OpenAI**: `OPENAI_API_KEY`
    - **Azure OpenAI**: `AZURE_OPENAI_API_KEY`
    - **Anthropic**: `ANTHROPIC_API_KEY`
    - **Open Router**: `OPENROUTER_API_KEY`
    - **Google Gemini**: `GOOGLE_API_KEY` or `GEMINI_API_KEY`
- **Max Tokens**: Specifies the maximum number of tokens (input + output) the AI can process in a single prompt.
- **API URL**: The endpoint URL used to connect to the AI provider’s API.
- **Deployment Name**: The name of the deployed model to use when calling Azure OpenAI.

### Provider Details

#### OpenAI Details

- **Sign up for API access**: [OpenAI Platform](https://platform.openai.com/signup)
- **Available models**: [OpenAI Models](https://platform.openai.com/docs/models/compare)
- **Pricing**: [OpenAI Pricing](https://platform.openai.com/docs/pricing)

#### Azure OpenAI Details

- **Sign up for Azure account**: [Azure Free Account](https://azure.microsoft.com/en-us/free/ai/)
- **Available models**: [Azure OpenAI Models](https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/models)
- **Pricing**: [Azure OpenAI Pricing](https://azure.microsoft.com/en-us/pricing/details/cognitive-services/openai-service/)

#### Anthropic Details

- **Sign up for API access**: [Anthropic Console](https://console.anthropic.com/)
- **Available models**: [Anthropic Models](https://docs.anthropic.com/claude/docs/models-overview)
- **Pricing**: [Anthropic Pricing](https://docs.anthropic.com/en/docs/about-claude/pricing)

#### Open Router Details

- **Sign up for API access**: [Open Router](https://openrouter.ai)
- **Available models**: 400+ models from 60+ providers (see [List Available Models](https://openrouter.ai/models)) — includes models from OpenAI, Anthropic, Google, open-source ones.
- **Pricing**:  Varies per model; example per-token rates (prompt vs completion) given in the model catalog.

#### Google Gemini Details

- **Sign up for API access**: [Google AI Studio](https://aistudio.google.com/)
- **Available models**: [Gemini Models](https://ai.google.dev/gemini-api/docs/models)
- **Pricing**: [Gemini Pricing](https://ai.google.dev/pricing)

#### Ollama Details

- **Download and install**: [Ollama Download](https://ollama.ai/)
- **Available models**: [Ollama Model Library](https://ollama.ai/library)
- **Pricing**: Models are free but will require powerful hardware to run locally

