export interface TinyUrl {
  id: number;
  shortCode: string;
  originalUrl: string;
  shortUrl: string;
  createdAt: string;
  hitCount: number;
  isPrivate: boolean;
}

export interface CreateUrlDto {
  originalUrl: string;
  isPrivate: boolean;
}
