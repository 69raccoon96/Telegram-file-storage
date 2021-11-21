import datetime
import uuid

from conftest import create_chat, create_sender
from postgres.basic import manager
from postgres.models.db_models import Chat, File, FileSender, FileTypeEnum


async def test_create_chat(db_manager):
    chat = await db_manager.create(model=Chat, Name='test name', ImageId=uuid.uuid4(), TelegramId=123)

    result = list(await manager.execute(Chat.select()))

    assert chat in result


async def test_contains_chat(db_manager):
    chat = await db_manager.create(model=Chat, Name='test name', ImageId=uuid.uuid4(), TelegramId=123)
    await create_chat(chat_name='mysupername')

    result = await db_manager.contains(chat)

    assert result


async def test_false_contains_chat(db_manager):
    chat = Chat(Name='test name', ImageId=uuid.uuid4(), TelegramId=123)
    result = await db_manager.contains(chat)

    assert not result


async def test_find_chat_by_telegram_id(db_manager):
    own_id = 228
    await create_chat(telegram_id=123)
    await create_chat(telegram_id=own_id)

    chat: Chat = await db_manager.get(model=Chat, TelegramId=own_id)

    assert chat.TelegramId == own_id


async def test_update_chat_information(db_manager):
    chat = await create_chat()
    chat_data_old = chat.as_dict()

    new_name = 'new name here'
    result = await db_manager.update(chat, Name=new_name)

    assert result

    result = await Chat.get(Id=chat.Id)
    new_chat_data = result.as_dict()
    assert new_chat_data != chat_data_old
    assert new_chat_data['Name'] == new_name


async def test_get_all_chats(db_manager):
    chat1 = await create_chat()
    chat2 = await create_chat(chat_name='abcd')

    result = await db_manager.get_all(Chat)

    assert chat1 in result
    assert chat2 in result


async def test_create_sender(db_manager):
    sender = await db_manager.create(model=FileSender, TelegramId=123, TelegramUserName='my name', FullName='full name')

    result = list(await manager.execute(FileSender.select()))

    assert sender in result


async def test_contains_sender(db_manager):
    sender = await db_manager.create(model=FileSender, TelegramId=123, TelegramUserName='my name', FullName='full name')
    await create_sender(name='mysupername')

    result = await db_manager.contains(sender)

    assert result


async def test_false_contains_sende(db_manager):
    sender = FileSender(
        TelegramId=123, TelegramUserName='my name', FullName='full name')
    result = await db_manager.contains(sender)

    assert not result


async def test_find_sender_by_telegram_id(db_manager):
    own_id = 228
    await create_sender(telegram_id=123)
    await create_sender(telegram_id=own_id)

    chat: FileSender = await db_manager.get(model=FileSender, TelegramId=own_id)

    assert chat.TelegramId == own_id


async def test_update_sender_information(db_manager):
    sender = await create_sender()
    chat_data_old = sender.as_dict()

    new_name = 'new name here'
    result = await db_manager.update(sender, FullName=new_name)

    assert result

    result = await FileSender.get(FullName=FileSender.FullName)
    new_chat_data = result.as_dict()
    assert new_chat_data != chat_data_old
    assert new_chat_data['FullName'] == new_name


async def test_get_all_senders(db_manager):
    sender1 = await create_sender()
    sender2 = await create_sender(name='abcd')

    result = await db_manager.get_all(FileSender)

    assert sender1 in result
    assert sender2 in result


async def test_create_file(db_manager):
    chat = await create_chat()
    sender = await create_sender()
    file = await db_manager.create(
        model=File,
        Name='name',
        Extension='extension',
        TypeId=FileTypeEnum.Document,
        UploadDate=datetime.datetime.now(),
        FileSenderId=sender.Id,
        ChatId=chat.Id,
    )

    result = list(await manager.execute(File.select()))

    assert file in result