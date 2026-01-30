import { api } from './api';

export interface GenerateEspelhoParams {
  userId: string;
  year: number;
  month: number;
}

export interface GenerateACJEFParams {
  year: number;
  month: number;
  userId?: string;
}

export interface GenerateAFDParams {
  startDate: string;
  endDate: string;
  userId?: string;
}

/**
 * Reports Service
 */
export const reportsService = {
  /**
   * Gera Espelho de Ponto (PDF) - Relatório Mensal
   * Admin visualiza de qualquer funcionário
   */
  async generateEspelho(params: GenerateEspelhoParams): Promise<Blob> {
    const queryParams = new URLSearchParams({
      userId: params.userId,
      year: params.year.toString(),
      month: params.month.toString()
    });

    const response = await fetch(
      `${import.meta.env.PUBLIC_API_URL}/Reports/espelho?${queryParams}`,
      {
        headers: {
          Authorization: `Bearer ${api.getAccessToken()}`
        }
      }
    );

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Erro ao gerar espelho de ponto');
    }

    return response.blob();
  },

  /**
   * Gera CRP - Comprovante de Registro de Ponto (PDF)
   * Usado para auditoria: admin visualiza comprovante de uma marcação específica
   */
  async generateCRP(timeRecordId: string): Promise<Blob> {
    const response = await fetch(
      `${import.meta.env.PUBLIC_API_URL}/Reports/crp/${timeRecordId}`,
      {
        headers: {
          Authorization: `Bearer ${api.getAccessToken()}`
        }
      }
    );

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Erro ao gerar comprovante');
    }

    return response.blob();
  },

  /**
   * Gera AFD - Arquivo Fonte de Dados (TXT)
   * Fiscalização MTE
   */
  async generateAFD(params: GenerateAFDParams): Promise<Blob> {
    const queryParams = new URLSearchParams({
      startDate: params.startDate,
      endDate: params.endDate
    });

    if (params.userId) {
      queryParams.append('userId', params.userId);
    }

    const response = await fetch(
      `${import.meta.env.PUBLIC_API_URL}/Reports/afd?${queryParams}`,
      {
        headers: {
          Authorization: `Bearer ${api.getAccessToken()}`
        }
      }
    );

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Erro ao gerar AFD');
    }

    return response.blob();
  },

  /**
   * Gera ACJEF - Arquivo Eletrônico de Jornada (JSON)
   * eSocial
   */
  async generateACJEF(params: GenerateACJEFParams): Promise<unknown> {
    const queryParams = new URLSearchParams({
      year: params.year.toString(),
      month: params.month.toString()
    });

    if (params.userId) {
      queryParams.append('userId', params.userId);
    }

    return api.get(`/Reports/acjef?${queryParams}`);
  },

  /**
   * Helper: Download blob as file
   */
  downloadBlob(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }
};