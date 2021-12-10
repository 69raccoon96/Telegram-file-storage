import typing
from io import BytesIO

from common.interactor.base_interactor import BaseInteractor
from telethon import TelegramClient
from telethon.events import NewMessage
from telethon.tl.custom import Message
from telethon.tl.types import Chat, User
from urlextract import URLExtract


class BaseHandler:

    def __init__(
        self,
        telegram_client: TelegramClient,
        base_interactor: BaseInteractor
    ):
        self.telegram_client = telegram_client
        self.loader_interactor = base_interactor

    def run(self):
        new_message_event = NewMessage()
        self.telegram_client.add_event_handler(
            self.__handle_new_message_event,
            new_message_event
        )

    def stop(self):
        self.telegram_client.remove_event_handler(
            self.__handle_new_message_event
        )

    async def __handle_new_message_event(self, event: NewMessage.Event):
        message: Message = event.message
        # is_valid = await self.is_valid_chat(message.chat_id)

        if message.chat_id < 0:
            # TODO: Сделать фильтрацию типов медиа
            if message.media:
                await self._handle_new_message_with_media(message)

            # TODO: Вынести в TelegramRepository и использовать его в BaseInteractor
            extractor = URLExtract()

            # TODO: В BaseInteractor метод должен называться has_urls
            if extractor.has_urls(message.message):
                urls = extractor.find_urls(message.message)
                await self._handle_new_message_with_urls(message, urls)

    async def _handle_new_message_with_media(self, message: Message):
        pass

    async def _handle_new_message_with_urls(self, message: Message, urls: list[str]):
        pass

    async def _is_valid_chat(self, chat_id: int) -> bool:
        return await self.loader_interactor.is_valid_chat(chat_id)

    async def _download_file(self, message: Message) -> BytesIO:
        file: BytesIO = typing.cast(BytesIO, await self.telegram_client.download_media(message, file=BytesIO()))
        return BytesIO(file.getvalue())

    async def _download_chat_photo(self, channel_id: int) -> BytesIO:
        photo_bytes = typing.cast(
            BytesIO,
            await self.telegram_client.download_profile_photo(entity=channel_id, file=BytesIO())
        )
        return BytesIO(photo_bytes.getvalue())

    async def _get_users_without_me(self, telegram_chat: Chat, me_id: int) -> list[User]:
        users: list[User] = await self.telegram_client.get_participants(telegram_chat)
        return list(filter(lambda user: user.id != me_id, users))
